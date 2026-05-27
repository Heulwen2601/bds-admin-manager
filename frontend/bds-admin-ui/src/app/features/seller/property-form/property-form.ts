import { CommonModule } from '@angular/common';
import { ChangeDetectorRef, Component, OnInit, inject } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { forkJoin, of } from 'rxjs';
import { CategoryApiService } from '../../../core/services/category-api';
import { SellerApiService } from '../../../core/services/seller-api.service';
import {
  Category,
  CreatePropertyRequest,
  Property,
  SavePropertyDraftRequest,
  UpdatePropertyRequest,
} from '../../../models';

@Component({
  selector: 'app-property-form',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, RouterLink],
  templateUrl: './property-form.html',
  styleUrl: './property-form.scss',
})
export class PropertyFormComponent implements OnInit {
  private fb = inject(FormBuilder);
  private route = inject(ActivatedRoute);
  private router = inject(Router);
  private sellerApi = inject(SellerApiService);
  private categoryApi = inject(CategoryApiService);
  private cdr = inject(ChangeDetectorRef);

  propertyId: string | null = null;
  loading = true;
  saving = false;
  errorMessage = '';
  draftSavedMessage = '';
  categories: Category[] = [];

  readonly form = this.fb.nonNullable.group({
    categoryGroup: ['', Validators.required],
    categoryId: ['', Validators.required],
    title: ['', [Validators.required, Validators.maxLength(300)]],
    description: [''],
    price: [0, [Validators.required, Validators.min(1)]],
    pricePerM2: [0],
    area: [0, [Validators.required, Validators.min(1)]],
    address: ['', [Validators.required, Validators.maxLength(300)]],
    ward: [''],
    district: [''],
    city: ['', [Validators.required, Validators.maxLength(100)]],
    projectName: [''],
    listingType: ['Standard'],
    listingCode: [''],
    expiredAt: [''],
  });

  readonly categoryGroups = [
    { value: 'for-sale', label: 'Nhà đất bán' },
    { value: 'for-rent', label: 'Nhà đất cho thuê' },
    { value: 'project-properties', label: 'Dự án' },
  ];

  ngOnInit(): void {
    this.propertyId = this.route.snapshot.paramMap.get('id');
    this.clearLegacyLocalDrafts();
    this.form.controls.price.valueChanges.subscribe(() => this.updatePricePerM2());
    this.form.controls.area.valueChanges.subscribe(() => this.updatePricePerM2());
    this.form.controls.categoryGroup.valueChanges.subscribe(() => {
      this.form.controls.categoryId.setValue('');
    });

    this.loadData();
  }

  get isEditMode(): boolean {
    return !!this.propertyId;
  }

  loadData(): void {
    this.loading = true;
    this.errorMessage = '';

    const requests = {
      categories: this.categoryApi.getAll(),
      property: this.propertyId ? this.sellerApi.getProperty(this.propertyId) : of(null),
    };

    forkJoin(requests).subscribe({
      next: ({ categories, property }) => {
        this.categories = categories.data ?? [];
        if (property?.data) {
          this.patchForm(property.data);
        }
        this.updatePricePerM2();
        this.loading = false;
        this.cdr.detectChanges();
      },
      error: (err) => {
        console.log(err);
        this.errorMessage = 'Không thể tải dữ liệu biểu mẫu từ backend.';
        this.loading = false;
        this.cdr.detectChanges();
      },
    });
  }

  submit(): void {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }

    this.saving = true;
    this.errorMessage = '';
    this.draftSavedMessage = '';
    const request = this.toRequest();
    const operation =
      this.isEditMode && this.propertyId
        ? this.sellerApi.updateProperty(this.propertyId, request)
        : this.sellerApi.createProperty(request);

    operation.subscribe({
      next: () => {
        this.router.navigate(['/seller/properties']);
      },
      error: (error) => {
        this.errorMessage =
          error.error?.message ||
          error.error?.title ||
          'Không thể lưu tin đăng. Vui lòng kiểm tra dữ liệu.';
        this.saving = false;
        this.cdr.detectChanges();
      },
    });
  }

  isInvalid(controlName: keyof typeof this.form.controls): boolean {
    const control = this.form.controls[controlName];
    return control.invalid && (control.dirty || control.touched);
  }

  get filteredCategories(): Category[] {
    const group = this.form.controls.categoryGroup.value;
    if (!group) {
      return [];
    }

    const hasHierarchy = this.categories.some((category) => !!category.parentId);
    return this.categories.filter((category) => {
      if (hasHierarchy && !category.parentId) {
        return false;
      }
      return this.categoryBelongsToGroup(category, group);
    });
  }

  private patchForm(property: Property): void {
    this.form.patchValue(
      {
        categoryGroup: this.resolveCategoryGroup(property),
        categoryId: property.categoryId,
        title: property.title,
        description: property.description ?? '',
        price: property.price / 1_000_000,
        pricePerM2: property.pricePerM2 ?? 0,
        area: property.area,
        address: property.address,
        ward: property.ward ?? '',
        district: property.district ?? '',
        city: property.city,
        projectName: property.projectName ?? '',
        listingType: property.listingType ?? 'Standard',
        listingCode: property.listingCode ?? '',
        expiredAt: property.expiredAt ? property.expiredAt.slice(0, 10) : '',
      },
      { emitEvent: false },
    );
  }

  private toRequest(): CreatePropertyRequest | UpdatePropertyRequest {
    const value = this.form.getRawValue();

    return {
      categoryId: value.categoryId,
      title: value.title.trim(),
      description: value.description?.trim() || undefined,
      price: Number(value.price) * 1_000_000,
      pricePerM2: value.pricePerM2 ? Number(value.pricePerM2) : undefined,
      area: Number(value.area),
      address: value.address.trim(),
      ward: value.ward?.trim() || undefined,
      district: value.district?.trim() || undefined,
      city: value.city.trim(),
      projectName: value.projectName?.trim() || undefined,
      listingType: value.listingType || 'Standard',
      expiredAt: value.expiredAt || undefined,
    };
  }

  private toDraftRequest(): SavePropertyDraftRequest {
    const value = this.form.getRawValue();
    const price = Number(value.price) || 0;
    const area = Number(value.area) || 0;

    return {
      categoryId: value.categoryId || undefined,
      title: value.title?.trim() || undefined,
      description: value.description?.trim() || undefined,
      price: price > 0 ? price * 1_000_000 : undefined,
      pricePerM2: value.pricePerM2 ? Number(value.pricePerM2) : undefined,
      area: area > 0 ? area : undefined,
      address: value.address?.trim() || undefined,
      ward: value.ward?.trim() || undefined,
      district: value.district?.trim() || undefined,
      city: value.city?.trim() || undefined,
      projectName: value.projectName?.trim() || undefined,
      listingType: value.listingType || 'Standard',
      expiredAt: value.expiredAt || undefined,
    };
  }

  private updatePricePerM2(): void {
    const price = (Number(this.form.controls.price.value) || 0) * 1_000_000;
    const area = Number(this.form.controls.area.value) || 0;
    const pricePerM2 = price > 0 && area > 0 ? Math.round(price / area) : 0;
    this.form.controls.pricePerM2.setValue(pricePerM2, { emitEvent: false });
  }

  saveDraft(): void {
    this.saving = true;
    this.errorMessage = '';
    this.draftSavedMessage = '';

    const request = this.toDraftRequest();
    const operation =
      this.isEditMode && this.propertyId
        ? this.sellerApi.updatePropertyDraft(this.propertyId, request)
        : this.sellerApi.savePropertyDraft(request);

    operation.subscribe({
      next: () => {
        this.router.navigate(['/seller/properties']);
      },
      error: (error) => {
        this.errorMessage =
          error.error?.message || error.error?.title || 'Không thể lưu bản nháp. Vui lòng thử lại.';
        this.saving = false;
        this.cdr.detectChanges();
      },
    });
  }

  private clearLegacyLocalDrafts(): void {
    localStorage.removeItem('seller-property-draft');
    localStorage.removeItem('seller-property-drafts');
    if (this.propertyId) {
      localStorage.removeItem(`seller-property-draft-${this.propertyId}`);
    }
  }

  private resolveCategoryGroup(property: Property): string {
    const category = this.categories.find((item) => item.id === property.categoryId);
    if (category) {
      for (const group of this.categoryGroups) {
        if (this.categoryBelongsToGroup(category, group.value)) {
          return group.value;
        }
      }
    }

    const normalizedGroup = property.categoryGroup?.trim().toLowerCase();
    if (!normalizedGroup) {
      return '';
    }

    if (['nhà đất bán', 'nha dat ban', 'for-sale'].includes(normalizedGroup)) {
      return 'for-sale';
    }
    if (['nhà đất cho thuê', 'nha dat cho thue', 'for-rent'].includes(normalizedGroup)) {
      return 'for-rent';
    }
    if (['dự án', 'du an', 'project-properties', 'project'].includes(normalizedGroup)) {
      return 'project-properties';
    }

    return '';
  }

  private categoryBelongsToGroup(category: Category, group: string): boolean {
    const effectiveCategory = this.getEffectiveCategory(category);
    const normalizedGroupName = effectiveCategory.groupName?.trim().toLowerCase() ?? '';
    const normalizedSlug = effectiveCategory.slug?.trim().toLowerCase() ?? '';

    switch (group) {
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

  private getEffectiveCategory(category: Category): Category {
    let currentCategory: Category | undefined = category;

    while (currentCategory) {
      if (currentCategory.groupName?.trim() || currentCategory.slug?.trim()) {
        return currentCategory;
      }

      const parentId: string | undefined = currentCategory.parentId;
      if (!parentId) {
        break;
      }

      currentCategory = this.categories.find((item) => item.id === parentId);
    }

    return category;
  }
}
