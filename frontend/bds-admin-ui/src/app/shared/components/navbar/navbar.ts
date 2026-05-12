import { Component, DestroyRef, ElementRef, HostListener, inject, OnInit } from '@angular/core';
import { Router, RouterLink, RouterLinkActive } from '@angular/router';
import { CommonModule } from '@angular/common';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { AuthService } from '../../../core/services/auth.service';
import { HomeRefreshService } from '../../../core/services/home-refresh.service';
import { CategoryApiService } from '../../../core/services/category-api';
import { Category } from '../../../models';

interface CategoryGroup {
  key: string;
  label: string;
  route: string;
  categories: Category[];
}

const PRIMARY_CATEGORY_GROUPS: CategoryGroup[] = [
  { key: 'for-sale', label: 'Nhà đất bán', route: '/for-sale', categories: [] },
  { key: 'for-rent', label: 'Cho thuê', route: '/for-rent', categories: [] },
  { key: 'project-properties', label: 'Dự án', route: '/projects', categories: [] }
];

@Component({
  selector: 'app-navbar',
  standalone: true,
  imports: [CommonModule, RouterLink, RouterLinkActive],
  templateUrl: './navbar.html',
  styleUrl: './navbar.scss'
})
export class NavbarComponent implements OnInit {
  authService = inject(AuthService);
  private router = inject(Router);
  private homeRefresh = inject(HomeRefreshService);
  private categoryApi = inject(CategoryApiService);
  private destroyRef = inject(DestroyRef);
  private elementRef = inject(ElementRef<HTMLElement>);

  categoryGroups: CategoryGroup[] = PRIMARY_CATEGORY_GROUPS.map(group => ({ ...group }));
  activeDropdown: string | null = null;
  dropdownTimeout: ReturnType<typeof setTimeout> | null = null;

  @HostListener('document:pointermove', ['$event'])
  onDocumentPointerMove(event: PointerEvent) {
    if (!this.activeDropdown) return;

    const target = event.target as Node | null;
    const targetElement = target instanceof Element ? target : target?.parentElement;
    const dropdownElement = targetElement?.closest('.dropdown');
    const isInsideDropdown = !!dropdownElement && this.elementRef.nativeElement.contains(dropdownElement);

    if (!isInsideDropdown) {
      this.closeDropdown();
    }
  }

  ngOnInit() {
    this.loadCategories();

    this.homeRefresh.refresh$
      .pipe(takeUntilDestroyed(this.destroyRef))
      .subscribe(() => this.loadCategories(true));
  }

  loadCategories(forceRefresh = false) {
    this.categoryApi.getAll(forceRefresh).subscribe({
      next: (response: any) => {
        if (response.success) {
          const categories: Category[] = response.data;
          this.categoryGroups = this.groupCategoriesByGroupName(categories);
        }
      },
      error: (error: any) => {
        console.error('Error loading categories:', error);
      }
    });
  }

  groupCategoriesByGroupName(categories: Category[]): CategoryGroup[] {
    const groups = new Map<string, CategoryGroup>();

    categories.forEach(category => {
      const groupName = category.groupName?.trim() || 'Khác';
      const key = this.resolveGroupKey(groupName);
      const existingGroup = groups.get(key);

      if (existingGroup) {
        existingGroup.categories.push(category);
      } else {
        groups.set(key, {
          key,
          label: this.resolveGroupLabel(groupName, key),
          route: this.resolveGroupRoute(key),
          categories: [category]
        });
      }
    });

    const orderedPrimaryGroups = PRIMARY_CATEGORY_GROUPS.map(primaryGroup => ({
      ...primaryGroup,
      categories: this.moveOtherPropertyToEnd(groups.get(primaryGroup.key)?.categories ?? [])
    }));

    const extraGroups = Array.from(groups.values())
      .filter(group => !PRIMARY_CATEGORY_GROUPS.some(primaryGroup => primaryGroup.key === group.key))
      .map(group => ({
        ...group,
        categories: this.moveOtherPropertyToEnd(group.categories)
      }));

    return [...orderedPrimaryGroups, ...extraGroups];
  }

  private moveOtherPropertyToEnd(categories: Category[]): Category[] {
    return [...categories].sort((a, b) => {
      const aIsOtherProperty = a.name.trim().toLowerCase() === 'other property';
      const bIsOtherProperty = b.name.trim().toLowerCase() === 'other property';

      if (aIsOtherProperty === bIsOtherProperty) return 0;
      return aIsOtherProperty ? 1 : -1;
    });
  }

  private resolveGroupKey(groupName: string): string {
    const normalizedGroupName = groupName.toLowerCase();

    if (normalizedGroupName.includes('sale')) return 'for-sale';
    if (normalizedGroupName.includes('rent')) return 'for-rent';
    if (normalizedGroupName.includes('project') || normalizedGroupName.includes('development')) {
      return 'project-properties';
    }

    return normalizedGroupName.replace(/[^a-z0-9]+/g, '-').replace(/^-|-$/g, '') || 'other';
  }

  private resolveGroupLabel(groupName: string, key: string): string {
    const primaryGroup = PRIMARY_CATEGORY_GROUPS.find(group => group.key === key);
    return primaryGroup?.label ?? groupName;
  }

  private resolveGroupRoute(key: string): string {
    const primaryGroup = PRIMARY_CATEGORY_GROUPS.find(group => group.key === key);
    return primaryGroup?.route ?? '/directory';
  }

  showDropdown(group: CategoryGroup) {
    if (this.dropdownTimeout) {
      clearTimeout(this.dropdownTimeout);
      this.dropdownTimeout = null;
    }
    this.activeDropdown = group.key;
  }

  hideDropdown() {
    this.dropdownTimeout = setTimeout(() => {
      this.closeDropdown();
    }, 100);
  }

  closeDropdown() {
    if (this.dropdownTimeout) {
      clearTimeout(this.dropdownTimeout);
      this.dropdownTimeout = null;
    }
    this.activeDropdown = null;
  }

  goHome(event: MouseEvent) {
    event.preventDefault();

    if (this.router.url.split('?')[0].split('#')[0] === '/') {
      this.homeRefresh.requestRefresh();
      return;
    }

    this.router.navigateByUrl('/');
  }

  logout() {
    this.authService.logout();
  }
}

