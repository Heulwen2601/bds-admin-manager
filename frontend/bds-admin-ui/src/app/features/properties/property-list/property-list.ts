import { Component, ElementRef, HostListener, OnInit, ViewChild, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { RouterLink } from '@angular/router';
import { ActivatedRoute } from '@angular/router';
import { combineLatest } from 'rxjs';
import { PropertyApiService } from '../../../core/services/property-api';
import { CategoryApiService } from '../../../core/services/category-api';
import { Property, Category, PropertyQueryParams } from '../../../models';

export interface FilterPreset {
  id: string;
  label: string;
  min?: number;
  max?: number;
}

const PRICE_PRESETS: FilterPreset[] = [
  { id: 'all', label: 'Tất cả khoảng giá' },
  { id: 'custom', label: 'Tùy chỉnh (nhập VNĐ)' },
  { id: 'lt500m', label: 'Dưới 500 triệu', max: 500_000_000 },
  { id: '500m-1b', label: '500 triệu — 1 tỷ', min: 500_000_000, max: 1_000_000_000 },
  { id: '1-3b', label: '1 — 3 tỷ', min: 1_000_000_000, max: 3_000_000_000 },
  { id: '3-5b', label: '3 — 5 tỷ', min: 3_000_000_000, max: 5_000_000_000 },
  { id: '5-7b', label: '5 — 7 tỷ', min: 5_000_000_000, max: 7_000_000_000 },
  { id: '7-10b', label: '7 — 10 tỷ', min: 7_000_000_000, max: 10_000_000_000 },
  { id: '10-15b', label: '10 — 15 tỷ', min: 10_000_000_000, max: 15_000_000_000 },
  { id: '15-20b', label: '15 — 20 tỷ', min: 15_000_000_000, max: 20_000_000_000 },
  { id: '20-30b', label: '20 — 30 tỷ', min: 20_000_000_000, max: 30_000_000_000 },
  { id: '30-40b', label: '30 — 40 tỷ', min: 30_000_000_000, max: 40_000_000_000 },
  { id: '40-60b', label: '40 — 60 tỷ', min: 40_000_000_000, max: 60_000_000_000 },
  { id: 'gt60b', label: 'Trên 60 tỷ', min: 60_000_000_000 },
];

const AREA_PRESETS: FilterPreset[] = [
  { id: 'all', label: 'Tất cả diện tích' },
  { id: 'custom', label: 'Tùy chỉnh (nhập m²)' },
  { id: 'lt30', label: 'Dưới 30 m²', max: 30 },
  { id: '30-50', label: '30 — 50 m²', min: 30, max: 50 },
  { id: '50-80', label: '50 — 80 m²', min: 50, max: 80 },
  { id: '80-100', label: '80 — 100 m²', min: 80, max: 100 },
  { id: '100-150', label: '100 — 150 m²', min: 100, max: 150 },
  { id: '150-200', label: '150 — 200 m²', min: 150, max: 200 },
  { id: '200-300', label: '200 — 300 m²', min: 200, max: 300 },
  { id: '300-500', label: '300 — 500 m²', min: 300, max: 500 },
  { id: 'gt500', label: 'Trên 500 m²', min: 500 },
];

@Component({
  selector: 'app-property-list',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterLink],
  templateUrl: './property-list.html',
  styleUrl: './property-list.scss',
})
export class PropertyListComponent implements OnInit {
  @ViewChild('filtersRoot') filtersRoot?: ElementRef<HTMLElement>;

  properties: Property[] = [];
  categories: Category[] = [];
  allCategories: Category[] = [];
  isLoading = false;
  pageTitle = 'Bất động sản';
  categoryGroup = '';
  allCategoriesLabel = 'Tất cả danh mục';

  searchQuery = '';
  selectedCategory = '';
  selectedCity = '';
  minPrice?: number;
  maxPrice?: number;
  minArea?: number;
  maxArea?: number;

  readonly pricePresets = PRICE_PRESETS;
  readonly areaPresets = AREA_PRESETS;

  pricePanelOpen = false;
  areaPanelOpen = false;
  pendingPricePresetId = 'all';
  pendingAreaPresetId = 'all';
  appliedPricePresetId = 'all';
  appliedAreaPresetId = 'all';
  priceDraftMin: number | null = null;
  priceDraftMax: number | null = null;
  areaDraftMin: number | null = null;
  areaDraftMax: number | null = null;

  currentPage = 1;
  totalPages = 1;

  private propertyApi = inject(PropertyApiService);
  private categoryApi = inject(CategoryApiService);
  private route = inject(ActivatedRoute);

  ngOnInit() {
    combineLatest([this.route.data, this.route.queryParams]).subscribe(([data, params]) => {
      this.categoryGroup = data['categoryGroup'] ?? '';
      this.pageTitle = data['title'] ?? 'Bất động sản';
      this.allCategoriesLabel = this.categoryGroup
        ? `Tất cả — ${this.pageTitle}`
        : 'Tất cả danh mục';
      this.selectedCategory = params['category'] ?? '';
      this.searchQuery = params['keyword'] ?? '';
      this.currentPage = 1;

      this.loadCategories();
      this.loadProperties();
    });
  }

  @HostListener('document:click', ['$event'])
  onDocumentClick(ev: MouseEvent): void {
    const root = this.filtersRoot?.nativeElement;
    if (root && !root.contains(ev.target as Node)) {
      this.pricePanelOpen = false;
      this.areaPanelOpen = false;
    }
  }

  get priceButtonLabel(): string {
    if (this.appliedPricePresetId === 'custom') {
      const a = this.minPrice;
      const b = this.maxPrice;
      if (a != null && b != null) return `Giá: ${a} – ${b} VNĐ`;
      if (a != null) return `Giá từ ${a} VNĐ`;
      if (b != null) return `Giá đến ${b} VNĐ`;
      return 'Khoảng giá · tùy chỉnh';
    }
    const p = this.pricePresets.find((x) => x.id === this.appliedPricePresetId);
    return p && p.id !== 'all' ? p.label : 'Khoảng giá';
  }

  get areaButtonLabel(): string {
    if (this.appliedAreaPresetId === 'custom') {
      const a = this.minArea;
      const b = this.maxArea;
      if (a != null && b != null) return `Diện tích: ${a} – ${b} m²`;
      if (a != null) return `Từ ${a} m²`;
      if (b != null) return `Đến ${b} m²`;
      return 'Diện tích · tùy chỉnh';
    }
    const p = this.areaPresets.find((x) => x.id === this.appliedAreaPresetId);
    return p && p.id !== 'all' ? p.label : 'Diện tích';
  }

  onPriceDraftInput(): void {
    this.pendingPricePresetId = 'custom';
  }

  onAreaDraftInput(): void {
    this.pendingAreaPresetId = 'custom';
  }

  openPricePanel(ev: MouseEvent): void {
    ev.stopPropagation();
    this.areaPanelOpen = false;
    this.syncDraftFromAppliedPrice();
    this.pricePanelOpen = !this.pricePanelOpen;
  }

  openAreaPanel(ev: MouseEvent): void {
    ev.stopPropagation();
    this.pricePanelOpen = false;
    this.syncDraftFromAppliedArea();
    this.areaPanelOpen = !this.areaPanelOpen;
  }

  private syncDraftFromAppliedPrice(): void {
    this.pendingPricePresetId = this.appliedPricePresetId;
    if (this.appliedPricePresetId === 'custom') {
      this.priceDraftMin = this.minPrice ?? null;
      this.priceDraftMax = this.maxPrice ?? null;
    } else {
      this.priceDraftMin = null;
      this.priceDraftMax = null;
    }
  }

  private syncDraftFromAppliedArea(): void {
    this.pendingAreaPresetId = this.appliedAreaPresetId;
    if (this.appliedAreaPresetId === 'custom') {
      this.areaDraftMin = this.minArea ?? null;
      this.areaDraftMax = this.maxArea ?? null;
    } else {
      this.areaDraftMin = null;
      this.areaDraftMax = null;
    }
  }

  selectPricePreset(id: string): void {
    this.pendingPricePresetId = id;
    if (id !== 'custom') {
      this.priceDraftMin = null;
      this.priceDraftMax = null;
    }
  }

  selectAreaPreset(id: string): void {
    this.pendingAreaPresetId = id;
    if (id !== 'custom') {
      this.areaDraftMin = null;
      this.areaDraftMax = null;
    }
  }

  resetPricePanel(): void {
    this.pendingPricePresetId = 'all';
    this.priceDraftMin = null;
    this.priceDraftMax = null;
  }

  resetAreaPanel(): void {
    this.pendingAreaPresetId = 'all';
    this.areaDraftMin = null;
    this.areaDraftMax = null;
  }

  applyPricePanel(ev: MouseEvent): void {
    ev.stopPropagation();
    this.applyPriceFromPending();
    this.pricePanelOpen = false;
    this.currentPage = 1;
    this.loadProperties();
  }

  applyAreaPanel(ev: MouseEvent): void {
    ev.stopPropagation();
    this.applyAreaFromPending();
    this.areaPanelOpen = false;
    this.currentPage = 1;
    this.loadProperties();
  }

  private applyPriceFromPending(): void {
    const dMin = this.priceDraftMin;
    const dMax = this.priceDraftMax;
    const typed = dMin != null || dMax != null;

    if (this.pendingPricePresetId === 'custom' || (this.pendingPricePresetId === 'all' && typed)) {
      this.appliedPricePresetId = 'custom';
      this.minPrice = dMin ?? undefined;
      this.maxPrice = dMax ?? undefined;
      return;
    }

    if (this.pendingPricePresetId === 'all') {
      this.appliedPricePresetId = 'all';
      this.minPrice = undefined;
      this.maxPrice = undefined;
      return;
    }

    this.appliedPricePresetId = this.pendingPricePresetId;
    const p = this.pricePresets.find((x) => x.id === this.pendingPricePresetId);
    this.minPrice = p?.min;
    this.maxPrice = p?.max;
  }

  private applyAreaFromPending(): void {
    const dMin = this.areaDraftMin;
    const dMax = this.areaDraftMax;
    const typed = dMin != null || dMax != null;

    if (this.pendingAreaPresetId === 'custom' || (this.pendingAreaPresetId === 'all' && typed)) {
      this.appliedAreaPresetId = 'custom';
      this.minArea = dMin ?? undefined;
      this.maxArea = dMax ?? undefined;
      return;
    }

    if (this.pendingAreaPresetId === 'all') {
      this.appliedAreaPresetId = 'all';
      this.minArea = undefined;
      this.maxArea = undefined;
      return;
    }

    this.appliedAreaPresetId = this.pendingAreaPresetId;
    const p = this.areaPresets.find((x) => x.id === this.pendingAreaPresetId);
    this.minArea = p?.min;
    this.maxArea = p?.max;
  }

  applyKeywordSearch(): void {
    this.currentPage = 1;
    this.loadProperties();
  }

  loadCategories() {
    this.categoryApi.getAll().subscribe({
      next: (response: any) => {
        if (response.success) {
          this.allCategories = response.data;
          this.categories = this.filterCategoriesByGroup(this.allCategories);

          if (
            this.selectedCategory &&
            !this.categories.some((category) => category.id === this.selectedCategory)
          ) {
            this.selectedCategory = '';
            this.loadProperties();
          }
        }
      },
      error: (error: any) => {
        console.error('Error loading categories:', error);
      },
    });
  }

  loadProperties() {
    this.isLoading = true;

    const params: PropertyQueryParams = {
      page: this.currentPage,
      pageSize: 12,
    };

    if (this.categoryGroup) params.categoryGroup = this.categoryGroup;
    if (this.searchQuery.trim()) params.keyword = this.searchQuery.trim();
    if (this.selectedCategory) params.categoryId = this.selectedCategory;
    if (this.selectedCity) params.city = this.selectedCity;
    if (this.minPrice != null) params.minPrice = this.minPrice;
    if (this.maxPrice != null) params.maxPrice = this.maxPrice;
    if (this.minArea != null) params.minArea = this.minArea;
    if (this.maxArea != null) params.maxArea = this.maxArea;

    this.propertyApi.getAll(params).subscribe({
      next: (response: any) => {
        if (response.success) {
          this.properties = response.data.items;
          this.totalPages = response.data.totalPages;
        }
        this.isLoading = false;
      },
      error: (error: any) => {
        console.error('Error loading properties:', error);
        this.isLoading = false;
      },
    });
  }

  onFilterChange() {
    this.currentPage = 1;
    this.loadProperties();
  }

  changePage(page: number) {
    if (page >= 1 && page <= this.totalPages) {
      this.currentPage = page;
      this.loadProperties();
    }
  }

  private filterCategoriesByGroup(categories: Category[]): Category[] {
    const filteredCategories = this.categoryGroup
      ? categories.filter((category) => this.isCategoryInActiveGroup(category))
      : categories;

    return this.moveOtherPropertyToEnd(filteredCategories);
  }

  private isCategoryInActiveGroup(category: Category): boolean {
    const groupName = category.groupName.trim().toLowerCase();

    if (this.categoryGroup === 'for-sale') return groupName.includes('sale');
    if (this.categoryGroup === 'for-rent') return groupName.includes('rent');
    if (this.categoryGroup === 'project-properties') {
      return groupName.includes('project') || groupName.includes('development');
    }

    return true;
  }

  private moveOtherPropertyToEnd(categories: Category[]): Category[] {
    return [...categories].sort((a, b) => {
      const aIsOtherProperty = a.name.trim().toLowerCase() === 'other property';
      const bIsOtherProperty = b.name.trim().toLowerCase() === 'other property';

      if (aIsOtherProperty === bIsOtherProperty) return 0;
      return aIsOtherProperty ? 1 : -1;
    });
  }
}
