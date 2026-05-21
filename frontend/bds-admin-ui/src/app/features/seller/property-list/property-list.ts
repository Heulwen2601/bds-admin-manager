import { CommonModule } from '@angular/common';
import { ChangeDetectorRef, Component, OnInit } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { RouterLink } from '@angular/router';
import { forkJoin } from 'rxjs';
import { CategoryApiService } from '../../../core/services/category-api';
import { SellerApiService } from '../../../core/services/seller-api.service';
import { Category, PagedResult, Property, PropertyQueryParams } from '../../../models';

@Component({
  selector: 'app-seller-property-list',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterLink],
  templateUrl: './property-list.html',
  styleUrl: './property-list.scss',
})
export class SellerPropertyListComponent implements OnInit {
  loading = true;
  errorMessage = '';
  actionMessage = '';

  properties: Property[] = [];
  categories: Category[] = [];
  allProperties: Property[] = [];
  localDrafts: Property[] = [];
  page = 1;
  pageSize = 10;
  totalCount = 0;
  totalPages = 1;

  filters: PropertyQueryParams = {
    keyword: '',
    categoryGroup: '',
    categoryId: '',
    listingType: '',
    status: '',
  };

  readonly statusTabs = [
    { value: '', label: 'Tất cả' },
    { value: 'Draft', label: 'Bản nháp' },
    { value: 'Pending', label: 'Chờ duyệt' },
    { value: 'Published', label: 'Đang hiển thị' },
    { value: 'Rejected', label: 'Không duyệt' },
  ];

  readonly categoryGroups = [
    { value: '', label: 'Tất cả nhóm tin' },
    { value: 'for-sale', label: 'Nhà đất bán' },
    { value: 'for-rent', label: 'Nhà đất cho thuê' },
    { value: 'project-properties', label: 'Dự án' },
  ];

  readonly listingTypes = [
    { value: '', label: 'Tất cả loại tin' },
    { value: 'Standard', label: 'Tin thường' },
    { value: 'VIP', label: 'VIP' },
    { value: 'Diamond', label: 'Diamond' },
  ];

  constructor(
    private sellerApi: SellerApiService,
    private categoryApi: CategoryApiService,
    private cdr: ChangeDetectorRef,
  ) {}

  ngOnInit(): void {
    this.loadInitialData();
  }

  loadInitialData(): void {
    this.loading = true;
    this.errorMessage = '';

    forkJoin({
      categories: this.categoryApi.getAll(),
      allProperties: this.sellerApi.getProperties(),
      result: this.sellerApi.searchProperties(this.buildQuery()),
    }).subscribe({
      next: ({ categories, allProperties, result }) => {
        this.categories = categories.data ?? [];
        this.loadLocalDrafts();
        this.allProperties = allProperties.data ?? [];
        this.applyResult(result.data);
        this.loading = false;
        this.cdr.detectChanges();
      },
      error: () => {
        this.errorMessage = 'Không thể tải danh sách tin đăng từ backend.';
        this.loading = false;
        this.cdr.detectChanges();
      },
    });
  }

  search(resetPage = true): void {
    if (resetPage) {
      this.page = 1;
    }

    this.loading = true;
    this.errorMessage = '';
    this.loadLocalDrafts();
    this.sellerApi.searchProperties(this.buildQuery()).subscribe({
      next: (response) => {
        this.applyResult(this.appendLocalDrafts(response.data));
        this.loading = false;
        this.cdr.detectChanges();
      },
      error: () => {
        this.errorMessage = 'Không thể lọc danh sách tin đăng.';
        this.loading = false;
        this.cdr.detectChanges();
      },
    });
  }

  resetFilters(): void {
    this.filters = {
      keyword: '',
      categoryGroup: '',
      categoryId: '',
      listingType: '',
      status: '',
    };
    this.search();
  }

  setStatus(status: string): void {
    this.filters.status = status;
    this.search();
  }

  setCategoryGroup(group: string): void {
    this.filters.categoryGroup = group;
    this.filters.categoryId = '';
    this.search();
  }

  get filteredCategories(): Category[] {
    if (!this.filters.categoryGroup) {
      return this.categories;
    }

    return this.categories.filter((category) =>
      this.categoryBelongsToGroup(category, this.filters.categoryGroup ?? ''),
    );
  }

  private categoryBelongsToGroup(category: Category, group: string): boolean {
    const normalizedGroupName = category.groupName?.trim().toLowerCase() ?? '';
    const normalizedSlug = category.slug?.trim().toLowerCase() ?? '';

    switch (group.trim().toLowerCase()) {
      case 'for-sale':
        return (
          normalizedGroupName === 'nhà đất bán' ||
          normalizedGroupName === 'nha dat ban' ||
          normalizedSlug.startsWith('ban-')
        );
      case 'for-rent':
        return (
          normalizedGroupName === 'nhà đất cho thuê' ||
          normalizedGroupName === 'nha dat cho thue' ||
          normalizedSlug.startsWith('cho-thue')
        );
      case 'project-properties':
        return (
          normalizedGroupName === 'dự án' ||
          normalizedGroupName === 'du an' ||
          normalizedSlug.startsWith('du-an')
        );
      default:
        return false;
    }
  }

  deleteProperty(property: Property): void {
    const confirmed = window.confirm(`Xóa tin "${property.title}"?`);
    if (!confirmed) {
      return;
    }

    this.sellerApi.deleteProperty(property.id).subscribe({
      next: () => {
        this.actionMessage = 'Đã xóa tin đăng.';
        this.allProperties = this.allProperties.filter((item) => item.id !== property.id);
        this.cdr.detectChanges();
        this.search(false);
      },
      error: () => {
        this.errorMessage = 'Không thể xóa tin đăng này.';
        this.cdr.detectChanges();
      },
    });
  }

  submitProperty(property: Property): void {
    this.sellerApi.submitPropertyForApproval(property.id).subscribe({
      next: () => {
        this.actionMessage = 'Đã gửi tin đăng chờ duyệt.';
        this.reloadCountsAndSearch();
      },
      error: () => {
        this.errorMessage = 'Chỉ tin nháp hoặc tin bị từ chối mới có thể gửi duyệt.';
        this.cdr.detectChanges();
      },
    });
  }

  goToPage(page: number): void {
    if (page < 1 || page > this.totalPages || page === this.page) {
      return;
    }

    this.page = page;
    this.search(false);
  }

  statusLabel(status?: string): string {
    switch (status) {
      case 'Published':
        return 'Đang hiển thị';
      case 'Pending':
        return 'Chờ duyệt';
      case 'Rejected':
        return 'Không duyệt';
      case 'Draft':
        return 'Bản nháp';
      default:
        return status || 'Không rõ';
    }
  }

  statusCount(status: string): number {
    const baseCount = this.allProperties.filter((property) => property.status === status).length;
    if (!status) {
      return baseCount + this.localDrafts.length;
    }

    if (status === 'Draft') {
      return baseCount + this.localDrafts.length;
    }

    return baseCount;
  }

  formatDate(value?: string): string {
    if (!value) {
      return 'Chưa cập nhật';
    }

    return new Intl.DateTimeFormat('vi-VN', {
      day: '2-digit',
      month: '2-digit',
      year: 'numeric',
    }).format(new Date(value));
  }

  formatPrice(value?: number): string {
    if (!value) {
      return 'Thỏa thuận';
    }

    if (value >= 1_000_000_000) {
      return `${this.formatNumber(value / 1_000_000_000)} tỷ`;
    }

    if (value >= 1_000_000) {
      return `${this.formatNumber(value / 1_000_000)} triệu`;
    }

    return new Intl.NumberFormat('vi-VN').format(value);
  }

  primaryImage(property: Property): string | null {
    const image = property.images?.find((item) => item.isPrimary) ?? property.images?.[0];
    return image?.url || image?.imageUrl || null;
  }

  canSubmit(property: Property): boolean {
    return property.status === 'Draft' || property.status === 'Rejected';
  }

  listingTypeLabel(value?: string): string {
    switch ((value || 'Standard').toLowerCase()) {
      case 'vip':
        return 'VIP';
      case 'diamond':
        return 'Diamond';
      case 'standard':
        return 'Thường';
      default:
        return value || 'Thường';
    }
  }

  private reloadCountsAndSearch(): void {
    forkJoin({
      allProperties: this.sellerApi.getProperties(),
      result: this.sellerApi.searchProperties(this.buildQuery()),
    }).subscribe({
      next: ({ allProperties, result }) => {
        this.allProperties = allProperties.data ?? [];
        this.applyResult(result.data);
        this.cdr.detectChanges();
      },
      error: () => {
        this.search(false);
      },
    });
  }

  private buildQuery(): PropertyQueryParams {
    return {
      ...this.filters,
      page: this.page,
      pageSize: this.pageSize,
    };
  }

  private applyResult(result: PagedResult<Property>): void {
    this.properties = result?.items ?? [];
    this.totalCount = result?.totalCount ?? 0;
    this.totalPages = Math.max(result?.totalPages ?? 1, 1);
    this.page = result?.page ?? this.page;
    this.pageSize = result?.pageSize ?? this.pageSize;
  }

  private appendLocalDrafts(result: PagedResult<Property>): PagedResult<Property> {
    if (this.filters.status !== 'Draft') {
      return result;
    }

    const draftItems = this.localDrafts;
    const combinedItems = [...draftItems, ...(result?.items ?? [])];
    const combinedTotal = (result?.totalCount ?? 0) + draftItems.length;
    const combinedPages = Math.max(
      Math.ceil(combinedTotal / (result?.pageSize ?? this.pageSize)),
      1,
    );

    return {
      ...result,
      items: combinedItems,
      totalCount: combinedTotal,
      totalPages: combinedPages,
    };
  }

  private loadLocalDrafts(): void {
    const raw = localStorage.getItem('seller-property-drafts');
    if (!raw) {
      this.localDrafts = [];
      return;
    }

    try {
      const drafts = JSON.parse(raw) as SavedDraft[];
      this.localDrafts = drafts.map((draft) => this.mapDraftToProperty(draft));
    } catch {
      this.localDrafts = [];
    }
  }

  private mapDraftToProperty(draft: SavedDraft): Property {
    const categoryId = String(draft.categoryId || '');
    const category = this.categories.find((item) => item.id === categoryId);

    return {
      id: String(draft.id || `draft-${Date.now()}`),
      userId: 'local',
      categoryId,
      categoryName: category?.name || String(draft.categoryGroup || 'Loại BĐS chưa chọn'),
      categoryGroup: category?.groupName || String(draft.categoryGroup || ''),
      title: String(draft.title || 'Tin nháp chưa có tiêu đề'),
      description: draft.description,
      price: Number(draft.price ?? 0),
      pricePerM2: draft.pricePerM2 ? Number(draft.pricePerM2) : undefined,
      area: Number(draft.area ?? 0),
      address: String(draft.address || ''),
      ward: draft.ward,
      district: draft.district,
      city: String(draft.city || ''),
      latitude: undefined,
      longitude: undefined,
      projectName: draft.projectName,
      status: 'Draft',
      rejectedReason: undefined,
      expiredAt: draft.expiredAt,
      listingCode: draft.listingCode,
      listingType: draft.listingType,
      bedrooms: undefined,
      bathrooms: undefined,
      seller: undefined,
      images: [],
      createdAt: String(draft.createdAt || new Date().toISOString()),
      updatedAt: String(draft.updatedAt || new Date().toISOString()),
    };
  }

  private formatNumber(value: number): string {
    return new Intl.NumberFormat('vi-VN', {
      maximumFractionDigits: 2,
    }).format(value);
  }
}

type SavedDraft = {
  id?: string;
  categoryId?: string;
  categoryGroup?: string;
  title?: string;
  description?: string;
  price?: number;
  pricePerM2?: number;
  area?: number;
  address?: string;
  ward?: string;
  district?: string;
  city?: string;
  projectName?: string;
  listingCode?: string;
  listingType?: string;
  expiredAt?: string;
  createdAt?: string;
  updatedAt?: string;
};
