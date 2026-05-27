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
    this.sellerApi.searchProperties(this.buildQuery()).subscribe({
      next: (response) => {
        this.applyResult(response.data);
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

    const hasHierarchy = this.categories.some((category) => !!category.parentId);
    return this.categories.filter((category) => {
      if (hasHierarchy && !category.parentId) {
        return false;
      }
      return this.categoryBelongsToGroup(category, this.filters.categoryGroup ?? '');
    });
  }

  private categoryBelongsToGroup(category: Category, group: string): boolean {
    const effectiveCategory = this.getEffectiveCategory(category);
    const normalizedGroupName = effectiveCategory.groupName?.trim().toLowerCase() ?? '';
    const normalizedSlug = effectiveCategory.slug?.trim().toLowerCase() ?? '';

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

  private getEffectiveCategory(category: Category): Category {
    let currentCategory: Category | undefined = category;

    while (currentCategory) {
      if (currentCategory.groupName?.trim()) {
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

  canSubmit(property: Property): boolean {
    return property.status === 'Draft' || property.status === 'Rejected';
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
    if (!status) {
      return this.allProperties.length;
    }

    return this.allProperties.filter((property) => property.status === status).length;
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

  private formatNumber(value: number): string {
    return new Intl.NumberFormat('vi-VN', {
      maximumFractionDigits: 2,
    }).format(value);
  }
}
