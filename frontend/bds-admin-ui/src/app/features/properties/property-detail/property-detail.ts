import { CommonModule } from '@angular/common';
import { Component, DestroyRef, OnInit, inject } from '@angular/core';
import { DomSanitizer, SafeResourceUrl } from '@angular/platform-browser';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { EMPTY, forkJoin, of } from 'rxjs';
import { catchError, finalize, switchMap } from 'rxjs/operators';
import { PropertyApiService } from '../../../core/services/property-api';
import { AuthService } from '../../../core/services/auth.service';
import {
  ApiResponse,
  CreateLeadRequest,
  Property,
  PropertyImage,
  PropertyQueryParams,
} from '../../../models';

@Component({
  selector: 'app-property-detail',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, RouterLink],
  templateUrl: './property-detail.html',
  styleUrl: './property-detail.scss',
})
export class PropertyDetailComponent implements OnInit {
  private readonly propertyApi = inject(PropertyApiService);
  private readonly route = inject(ActivatedRoute);
  private readonly router = inject(Router);
  private readonly authService = inject(AuthService);
  private readonly destroyRef = inject(DestroyRef);
  private readonly fb = inject(FormBuilder);
  private readonly sanitizer = inject(DomSanitizer);

  readonly fallbackImage =
    'https://images.unsplash.com/photo-1600566753086-00f18fb6b3ea?auto=format&fit=crop&w=1400&q=80';

  propertyId = '';
  property?: Property;
  images: PropertyImage[] = [];
  relatedProperties: Property[] = [];
  activeImageIndex = 0;
  isLoading = true;
  errorMessage = '';
  leadStatus: 'idle' | 'submitting' | 'success' | 'error' = 'idle';
  leadMessage = '';
  shareMessage = '';

  mapUrl: SafeResourceUrl | null = null;
  mapLoading = false;
  mapError = '';

  leadForm = this.fb.group({
    fullName: ['', [Validators.required, Validators.maxLength(100)]],
    phone: [
      '',
      [Validators.required, Validators.maxLength(20), Validators.pattern(/^[0-9+()\s.-]{8,20}$/)],
    ],
    email: ['', [Validators.email, Validators.maxLength(150)]],
    message: ['', [Validators.maxLength(1000)]],
  });

  ngOnInit(): void {
    this.route.paramMap
      .pipe(
        takeUntilDestroyed(this.destroyRef),
        switchMap((params) => {
          const id = params.get('id');
          this.resetState(id ?? '');

          if (!id) {
            this.isLoading = false;
            this.errorMessage = 'Không tìm thấy mã tin đăng.';
            return EMPTY;
          }

          return forkJoin({
            propertyResponse: this.propertyApi.getById(id),
            imageResponse: this.propertyApi.getImages(id).pipe(
              catchError(() =>
                of({
                  success: true,
                  data: [],
                  message: '',
                } as ApiResponse<PropertyImage[]>),
              ),
            ),
          }).pipe(finalize(() => (this.isLoading = false)));
        }),
      )
      .subscribe({
        next: ({ propertyResponse, imageResponse }) => {
          if (!propertyResponse.success || !propertyResponse.data) {
            this.errorMessage = propertyResponse.message || 'Không tải được tin đăng.';
            return;
          }

          this.property = propertyResponse.data;
          this.images = this.normalizeImages([
            ...(propertyResponse.data.images ?? []),
            ...(imageResponse.data ?? []),
          ]);
          this.setDefaultLeadMessage(propertyResponse.data);
          void this.loadMap(propertyResponse.data);
          this.loadRelatedProperties(propertyResponse.data);
        },
        error: () => {
          this.isLoading = false;
          this.errorMessage = 'Tin đăng không tồn tại hoặc chưa được công khai.';
        },
      });
  }

  get activeImageSrc(): string {
    const image = this.images[this.activeImageIndex];
    return image ? this.getImageSource(image) : this.fallbackImage;
  }

  get hasMultipleImages(): boolean {
    return this.images.length > 1;
  }

  get propertyLocation(): string {
    if (!this.property) return '';
    return [this.property.address, this.property.ward, this.property.district, this.property.city]
      .filter(Boolean)
      .join(', ');
  }

  get shortLocation(): string {
    if (!this.property) return '';
    return [this.property.district, this.property.city].filter(Boolean).join(', ');
  }

  get sellerName(): string {
    return this.property?.seller?.displayName || 'Chuyên viên tư vấn';
  }

  get sellerInitials(): string {
    const parts = this.sellerName.trim().split(/\s+/).filter(Boolean);
    return (parts.length ? parts.slice(-2) : ['BDS'])
      .map((part) => part.charAt(0).toUpperCase())
      .join('');
  }

  get sellerPhone(): string {
    return this.property?.seller?.phone || '';
  }

  get sellerPhoneHref(): string {
    return this.sellerPhone ? `tel:${this.sellerPhone.replace(/\s/g, '')}` : '';
  }

  get isAuthenticated(): boolean {
    return this.authService.isAuthenticated();
  }

  get hasFilledContactInfo(): boolean {
    const fullNameControl = this.leadForm.get('fullName');
    const phoneControl = this.leadForm.get('phone');
    return !!(fullNameControl?.valid && phoneControl?.valid);
  }

  get canViewSellerPhone(): boolean {
    return this.isAuthenticated && this.hasFilledContactInfo;
  }

  confirmCall(): void {
    if (!this.canViewSellerPhone) {
      window.alert(
        'Vui lòng điền đầy đủ họ tên và số điện thoại để xem số điện thoại và gọi người đăng.',
      );
      return;
    }

    const confirmed = window.confirm(
      `Xác nhận gọi ${this.sellerName} theo số ${this.sellerPhone}?`,
    );
    if (confirmed && this.sellerPhoneHref) {
      window.location.href = this.sellerPhoneHref;
    }
  }

  get descriptionParagraphs(): string[] {
    const description = this.property?.description?.trim();
    if (!description)
      return ['Tin đăng chưa có mô tả chi tiết. Vui lòng gửi yêu cầu để được tư vấn thêm.'];
    return description
      .split(/\r?\n/)
      .map((line) => line.trim())
      .filter(Boolean);
  }

  get overviewItems(): Array<{ label: string; value: string }> {
    const property = this.property;
    if (!property) return [];

    const pricePerM2 =
      property.pricePerM2 ?? (property.area > 0 ? property.price / property.area : undefined);

    return [
      { label: 'Mức giá', value: this.formatPrice(property.price) },
      { label: 'Diện tích', value: `${this.formatNumber(property.area)} m²` },
      {
        label: 'Giá/m²',
        value: pricePerM2 ? `${this.formatPrice(pricePerM2)}/m²` : 'Đang cập nhật',
      },
      { label: 'Loại tin', value: this.formatListingType(property.listingType) },
    ];
  }

  get detailRows(): Array<{ label: string; value: string }> {
    const property = this.property;
    if (!property) return [];

    const rows = [
      { label: 'Địa chỉ', value: this.propertyLocation },
      { label: 'Dự án', value: property.projectName },
      { label: 'Danh mục', value: property.categoryName },
      { label: 'Mã tin', value: property.listingCode },
      { label: 'Ngày đăng', value: this.formatDate(property.createdAt) },
      { label: 'Cập nhật', value: this.formatDate(property.updatedAt) },
      { label: 'Trạng thái', value: this.formatStatus(property.status) },
    ];

    return rows
      .filter((row) => !!row.value)
      .map((row) => ({ label: row.label, value: row.value as string }));
  }

  selectImage(index: number): void {
    if (index >= 0 && index < this.images.length) this.activeImageIndex = index;
  }

  previousImage(): void {
    if (!this.hasMultipleImages) return;
    this.activeImageIndex =
      this.activeImageIndex === 0 ? this.images.length - 1 : this.activeImageIndex - 1;
  }

  nextImage(): void {
    if (!this.hasMultipleImages) return;
    this.activeImageIndex =
      this.activeImageIndex === this.images.length - 1 ? 0 : this.activeImageIndex + 1;
  }

  submitLead(): void {
    if (!this.propertyId || this.leadForm.invalid) {
      this.leadForm.markAllAsTouched();
      return;
    }

    this.leadStatus = 'submitting';
    this.leadMessage = '';

    this.propertyApi
      .submitLead(this.propertyId, this.buildLeadPayload())
      .pipe(takeUntilDestroyed(this.destroyRef))
      .subscribe({
        next: (response) => {
          if (!response.success) {
            this.leadStatus = 'error';
            this.leadMessage = response.message || 'Không gửi được yêu cầu.';
            return;
          }

          this.leadStatus = 'success';
          this.leadMessage = 'Đã gửi thông tin. Tư vấn viên sẽ liên hệ với bạn sớm.';
          this.leadForm.reset();
          if (this.property) this.setDefaultLeadMessage(this.property);
        },
        error: () => {
          this.leadStatus = 'error';
          this.leadMessage = 'Không gửi được yêu cầu. Vui lòng kiểm tra lại thông tin.';
        },
      });
  }

  isFieldInvalid(name: 'fullName' | 'phone' | 'email' | 'message'): boolean {
    const control = this.leadForm.get(name);
    return !!control && control.invalid && (control.dirty || control.touched);
  }

  getImageSource(image?: PropertyImage): string {
    return image?.imageUrl || image?.url || '';
  }

  getPropertyImage(property: Property): string {
    const primary = property.images?.find((image) => image.isPrimary);
    return this.getImageSource(primary ?? property.images?.[0]) || this.fallbackImage;
  }

  getPropertyLocation(property: Property): string {
    return [property.district, property.city].filter(Boolean).join(', ') || property.city;
  }

  formatPrice(value?: number | null): string {
    if (value == null || value <= 0) return 'Thỏa thuận';
    if (value >= 1_000_000_000) return `${this.formatDecimal(value / 1_000_000_000)} tỷ`;
    if (value >= 1_000_000) return `${this.formatDecimal(value / 1_000_000)} triệu`;
    return `${this.formatNumber(value)} đ`;
  }

  formatNumber(value?: number | null): string {
    if (value == null) return 'Đang cập nhật';
    return new Intl.NumberFormat('vi-VN', { maximumFractionDigits: 1 }).format(value);
  }

  formatDate(value?: string): string {
    if (!value) return '';
    return new Intl.DateTimeFormat('vi-VN', {
      day: '2-digit',
      month: '2-digit',
      year: 'numeric',
    }).format(new Date(value));
  }

  formatListingType(type?: string): string {
    const normalized = type?.trim().toLowerCase();
    if (!normalized) return 'Tin thường';
    if (normalized === 'diamond') return 'Tin kim cương';
    if (normalized === 'vip') return 'Tin VIP';
    if (normalized === 'standard') return 'Tin thường';
    return type ?? 'Tin thường';
  }

  formatStatus(status?: string): string {
    const normalized = status?.trim().toLowerCase();
    if (normalized === 'published') return 'Đang hiển thị';
    if (normalized === 'pending') return 'Chờ duyệt';
    if (normalized === 'draft') return 'Bản nháp';
    if (normalized === 'rejected') return 'Bị từ chối';
    return status || 'Đang cập nhật';
  }

  shareProperty(): void {
    const url = window.location.href;
    const title = this.property?.title ?? 'Tin bất động sản';

    if (navigator.share) {
      void navigator.share({ title, url });
      return;
    }

    void navigator.clipboard?.writeText(url);
    this.shareMessage = 'Đã sao chép liên kết';
    window.setTimeout(() => (this.shareMessage = ''), 2200);
  }

  private resetState(id: string): void {
    this.propertyId = id;
    this.property = undefined;
    this.images = [];
    this.relatedProperties = [];
    this.activeImageIndex = 0;
    this.errorMessage = '';
    this.leadStatus = 'idle';
    this.leadMessage = '';
    this.isLoading = true;
  }

  private normalizeImages(images: PropertyImage[]): PropertyImage[] {
    const uniqueImages = new Map<string, PropertyImage>();

    for (const image of images) {
      const source = this.getImageSource(image);
      if (!source) continue;
      uniqueImages.set(image.id || source, image);
    }

    return Array.from(uniqueImages.values()).sort((a, b) => {
      const primaryOrder = Number(b.isPrimary) - Number(a.isPrimary);
      if (primaryOrder !== 0) return primaryOrder;
      return (a.sortOrder ?? 0) - (b.sortOrder ?? 0);
    });
  }

  private setDefaultLeadMessage(property: Property): void {
    this.leadForm.patchValue({
      message: `Tôi quan tâm đến tin "${property.title}". Vui lòng liên hệ tư vấn thêm.`,
    });
  }

  private loadRelatedProperties(property: Property): void {
    const params: PropertyQueryParams = {
      page: 1,
      pageSize: 7,
      city: property.city,
      categoryId: property.categoryId,
    };

    this.propertyApi
      .getAll(params)
      .pipe(takeUntilDestroyed(this.destroyRef))
      .subscribe({
        next: (response) => {
          if (!response.success) return;
          this.relatedProperties = response.data.items
            .filter((item) => item.id !== property.id)
            .slice(0, 6);
        },
      });
  }

  private async loadMap(property: Property): Promise<void> {
    this.mapError = '';
    this.mapUrl = null;
    this.mapLoading = true;

    const coords =
      property.latitude != null && property.longitude != null
        ? {
            lat: property.latitude,
            lon: property.longitude,
          }
        : undefined;

    try {
      const latLon = coords
        ? coords
        : await this.geocodeAddress([
            property.address,
            property.ward,
            property.district,
            property.city,
          ]);

      if (!latLon) {
        this.mapError = 'Không tìm thấy vị trí bản đồ cho địa chỉ này.';
        return;
      }

      const lat = Number(latLon.lat);
      const lon = Number(latLon.lon);
      const delta = 0.003;
      const minLon = lon - delta;
      const minLat = lat - delta;
      const maxLon = lon + delta;
      const maxLat = lat + delta;
      const mapSrc = `https://www.openstreetmap.org/export/embed.html?bbox=${minLon}%2C${minLat}%2C${maxLon}%2C${maxLat}&layer=mapnik&marker=${lat}%2C${lon}`;

      this.mapUrl = this.sanitizer.bypassSecurityTrustResourceUrl(mapSrc);
    } catch (error) {
      this.mapError = 'Không thể hiển thị bản đồ. Vui lòng thử lại sau.';
      this.mapUrl = null;
    } finally {
      this.mapLoading = false;
    }
  }

  private async geocodeAddress(
    parts: Array<string | undefined>,
  ): Promise<{ lat: string; lon: string } | null> {
    const queries = [
      parts.filter(Boolean).join(', '),
      [parts[0], parts[2], parts[3]].filter(Boolean).join(', '),
      [parts[2], parts[3]].filter(Boolean).join(', '),
      [parts[3]].filter(Boolean).join(', '),
    ].filter(Boolean) as string[];

    for (const queryText of queries) {
      const query = encodeURIComponent(`${queryText}, Việt Nam`);
      const response = await fetch(
        `https://nominatim.openstreetmap.org/search?format=json&limit=1&countrycodes=vn&q=${query}`,
        {
          headers: {
            'Accept-Language': 'vi',
            'User-Agent': 'BdsAdminManager/1.0',
          },
        },
      );

      if (!response.ok) {
        continue;
      }

      const results = (await response.json()) as Array<{ lat: string; lon: string }>;
      if (results.length) {
        return results[0];
      }
    }

    return null;
  }

  private buildLeadPayload(): CreateLeadRequest {
    const value = this.leadForm.getRawValue();
    const payload: CreateLeadRequest = {
      fullName: (value.fullName ?? '').trim(),
      phone: (value.phone ?? '').trim(),
    };
    const email = (value.email ?? '').trim();
    const message = (value.message ?? '').trim();

    if (email) payload.email = email;
    if (message) payload.message = message;

    return payload;
  }

  private formatDecimal(value: number): string {
    return new Intl.NumberFormat('vi-VN', {
      maximumFractionDigits: 2,
    }).format(value);
  }
}
