import { Component, DestroyRef, ElementRef, OnInit, ViewChild, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { CategoryApiService } from '../../core/services/category-api';
import { HomeRefreshService } from '../../core/services/home-refresh.service';
import { PropertyApiService } from '../../core/services/property-api';
import { Category, Property } from '../../models';

interface HomeProject {
  name: string;
  location: string;
  imageUrl: string;
  units: string;
}

interface LocationSpotlight {
  name: string;
  count: string;
  imageUrl: string;
}

interface NewsItem {
  title: string;
  category: string;
  imageUrl: string;
}

interface BusinessLogo {
  name: string;
  logoUrl: string;
  websiteUrl?: string;
}

interface SupportTool {
  title: string;
  description: string;
  icon: string;
}

@Component({
  selector: 'app-home',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterLink],
  templateUrl: './home.html',
  styleUrl: './home.scss',
})
export class Home implements OnInit {
  @ViewChild('businessCarousel') businessCarousel?: ElementRef<HTMLElement>;

  searchTabs = [
    { key: 'sale', label: 'Mua bán', route: '/for-sale' },
    { key: 'rent', label: 'Cho thuê', route: '/for-rent' },
    { key: 'project', label: 'Dự án', route: '/projects' },
  ];
  activeSearchTab = this.searchTabs[0];
  searchQuery = '';
  categories: Category[] = [];
  properties: Property[] = [];
  isLoading = false;
  categoriesLoading = false;
  errorMessage = '';
  featuredNews: NewsItem[] = [
    {
      title: 'Thị trường bất động sản phía Nam ghi nhận nguồn cung mới',
      category: 'Tin nổi bật',
      imageUrl:
        'https://images.unsplash.com/photo-1486406146926-c627a92ad1ab?auto=format&fit=crop&w=900&q=80',
    },
    {
      title: 'Những lưu ý khi mua nhà lần đầu trong giai đoạn hiện nay',
      category: 'Tư vấn',
      imageUrl:
        'https://images.unsplash.com/photo-1560518883-ce09059eeffa?auto=format&fit=crop&w=900&q=80',
    },
    {
      title: 'Căn hộ gần trung tâm tiếp tục được người mua quan tâm',
      category: 'Căn hộ',
      imageUrl:
        'https://images.unsplash.com/photo-1494526585095-c41746248156?auto=format&fit=crop&w=900&q=80',
    },
  ];
  marketNews = [
    'Chính thức ra mắt phân khu ven sông tại khu đô thị mới',
    'Hạ tầng giao thông tạo động lực cho bất động sản khu Đông',
    'Người mua ưu tiên sản phẩm có pháp lý rõ ràng',
    'Nhà phố thương mại hút khách nhờ khai thác dòng tiền',
    'Giá thuê căn hộ tại các đô thị lớn duy trì ổn định',
  ];
  featuredProjects: HomeProject[] = [
    {
      name: 'Sol Garden',
      location: 'Hai Phong',
      imageUrl:
        'https://images.unsplash.com/photo-1600607687939-ce8a6c25118c?auto=format&fit=crop&w=900&q=80',
      units: '230 căn',
    },
    {
      name: 'Imperial Sky Park',
      location: 'Hà Nội',
      imageUrl:
        'https://images.unsplash.com/photo-1600566753190-17f0baa2a6c3?auto=format&fit=crop&w=900&q=80',
      units: '520 căn',
    },
    {
      name: 'Lakeview Heights',
      location: 'TP. Thu Duc',
      imageUrl:
        'https://images.unsplash.com/photo-1600607687920-4e2a09cf159d?auto=format&fit=crop&w=900&q=80',
      units: '180 căn',
    },
    {
      name: 'Lasso Saigon',
      location: 'TP. Hồ Chí Minh',
      imageUrl:
        'https://images.unsplash.com/photo-1600585154340-be6161a56a0c?auto=format&fit=crop&w=900&q=80',
      units: '310 căn',
    },
  ];
  locationSpotlights: LocationSpotlight[] = [
    {
      name: 'TP. Ho Chi Minh',
      count: '8.476 tin dang',
      imageUrl:
        'https://images.unsplash.com/photo-1583417319070-4a69db38a482?auto=format&fit=crop&w=900&q=80',
    },
    {
      name: 'Hà Nội',
      count: '7.247 tin dang',
      imageUrl:
        'https://images.unsplash.com/photo-1528127269322-539801943592?auto=format&fit=crop&w=900&q=80',
    },
    {
      name: 'Đà Nẵng',
      count: '1.575 tin dang',
      imageUrl:
        'https://images.unsplash.com/photo-1559592413-7cec4d0cae2b?auto=format&fit=crop&w=900&q=80',
    },
    {
      name: 'Bình Dương',
      count: '1.498 tin dang',
      imageUrl:
        'https://images.unsplash.com/photo-1500530855697-b586d89ba3ee?auto=format&fit=crop&w=900&q=80',
    },
    {
      name: 'Đồng Nai',
      count: '1.210 tin dang',
      imageUrl:
        'https://images.unsplash.com/photo-1564013799919-ab600027ffc6?auto=format&fit=crop&w=900&q=80',
    },
  ];
  supportTools: SupportTool[] = [
    { title: 'Xem tuổi xây nhà', description: 'Tra cứu tuổi hợp để khởi công', icon: '01' },
    { title: 'Chi phí làm nhà', description: 'Ước tính ngân sách xây dựng', icon: '02' },
    { title: 'Tính lãi suất', description: 'Lập kế hoạch dòng tiền vay mua nhà', icon: '03' },
    { title: 'Tư vấn phong thủy', description: 'Gợi ý hướng nhà và bố trí cơ bản', icon: '04' },
  ];
  businesses: BusinessLogo[] = [
    {
      name: 'Gamuda Land',
      logoUrl: '/logo/businesses/gamuda-land.png',
      websiteUrl: 'https://gamudaland.com.vn/',
    },
    {
      name: 'Phu Dong SkyOne',
      logoUrl: '/logo/businesses/phu-dong-sky-one.png',
      websiteUrl: 'https://skyone.phudonggroup.com/',
    },
    {
      name: 'Cat Tuong Group',
      logoUrl: '/logo/businesses/cat-tuong-group.png',
      websiteUrl: 'https://cattuonggroup.vn/',
    },
    {
      name: 'Kim Oanh Group',
      logoUrl: '/logo/businesses/kim-oanh-group.jpg',
      websiteUrl: 'https://kimoanhgroup.vn/',
    },
    {
      name: 'Hunter Land',
      logoUrl: '/logo/businesses/hunter-land.png',
      websiteUrl: 'https://hunterland.com.vn/',
    },
    {
      name: 'SP Home',
      logoUrl: '/logo/businesses/sp-home.jpg',
    },
  ];

  private categoryApi = inject(CategoryApiService);
  private propertyApi = inject(PropertyApiService);
  private homeRefresh = inject(HomeRefreshService);
  private router = inject(Router);
  private destroyRef = inject(DestroyRef);

  ngOnInit(): void {
    this.homeRefresh.refresh$
      .pipe(takeUntilDestroyed(this.destroyRef))
      .subscribe(() => this.loadHomeData());

    this.loadHomeData();
  }

  loadHomeData(): void {
    this.isLoading = true;
    this.categoriesLoading = true;

    this.categoryApi.getAll().subscribe({
      next: (response: any) => {
        if (response.success) {
          this.categories = response.data;
        }
        this.categoriesLoading = false;
      },
      error: (error: any) => {
        console.error('Error loading categories:', error);
        this.categoriesLoading = false;
      },
    });

    this.propertyApi.getAll({ pageSize: 6 }).subscribe({
      next: (response: any) => {
        if (response.success) {
          this.properties = response.data.items;
        }
        this.isLoading = false;
      },
      error: (error: any) => {
        this.errorMessage = 'Không tải được danh sách bất động sản.';
        this.isLoading = false;
        console.error('Error loading properties:', error);
      },
    });
  }

  get recommendedProperties(): Property[] {
    return this.properties.slice(0, 8);
  }

  get projectCategories(): Category[] {
    return this.categories
      .filter((category) => {
        const groupName = category.groupName?.toLowerCase() ?? '';
        return groupName.includes('project') || groupName.includes('development');
      })
      .slice(0, 8);
  }

  get primaryNews(): NewsItem {
    return this.featuredNews[0];
  }

  get secondaryNews(): NewsItem[] {
    return this.featuredNews.slice(1);
  }

  getPropertyImage(property: Property): string {
    return (
      property.images?.find((image) => image.isPrimary)?.imageUrl ??
      property.images?.[0]?.imageUrl ??
      'https://images.unsplash.com/photo-1600566753086-00f18fb6b3ea?auto=format&fit=crop&w=900&q=80'
    );
  }

  getPropertyLocation(property: Property): string {
    return [property.district, property.city].filter(Boolean).join(', ') || property.address;
  }

  getProjectRoute(category: Category): unknown[] {
    return ['/projects'];
  }

  selectSearchTab(tab: { key: string; label: string; route: string }): void {
    this.activeSearchTab = tab;
  }

  submitSearch(): void {
    const keyword = this.searchQuery.trim();

    this.router.navigate([this.activeSearchTab.route], {
      queryParams: keyword ? { keyword } : {},
    });
  }

  scrollBusinesses(direction: 'left' | 'right'): void {
    const carousel = this.businessCarousel?.nativeElement;
    if (!carousel) return;

    const firstLogo = carousel.querySelector<HTMLElement>('.business-logo');
    if (!firstLogo) return;

    const carouselStyles = getComputedStyle(carousel);
    const gap = parseFloat(carouselStyles.gap || carouselStyles.columnGap || '0');
    const safeGap = Number.isFinite(gap) ? gap : 0;
    const itemWidth = firstLogo.offsetWidth + safeGap;
    const visibleCount =
      itemWidth > 0 ? Math.max(1, Math.floor(carousel.clientWidth / itemWidth)) : 1;
    const amount = itemWidth * visibleCount || carousel.clientWidth;

    carousel.scrollBy({ left: direction === 'right' ? amount : -amount, behavior: 'smooth' });
  }
}
