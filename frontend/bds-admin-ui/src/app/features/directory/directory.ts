import { CommonModule } from '@angular/common';
import { Component } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { RouterLink } from '@angular/router';

type DirectoryTab = 'brokers' | 'businesses';

interface BrokerProfile {
  name: string;
  role: string;
  company: string;
  city: string;
  specialty: string;
  experience: string;
  listings: number;
  phone: string;
  email: string;
  avatarUrl: string;
}

interface BusinessProfile {
  name: string;
  type: string;
  city: string;
  description: string;
  projects: number;
  agents: number;
  logoUrl: string;
  websiteUrl?: string;
}

@Component({
  selector: 'app-directory',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterLink],
  templateUrl: './directory.html',
  styleUrl: './directory.scss',
})
export class DirectoryComponent {
  readonly allCitiesLabel = 'Tất cả khu vực';
  activeTab: DirectoryTab = 'brokers';
  searchQuery = '';
  selectedCity = this.allCitiesLabel;

  readonly cities = [
    this.allCitiesLabel,
    'TP. Hồ Chí Minh',
    'Hà Nội',
    'Bình Dương',
    'Đồng Nai',
    'Đà Nẵng',
  ];

  readonly brokers: BrokerProfile[] = [
    {
      name: 'Nguyễn Minh Anh',
      role: 'Môi giới căn hộ cao cấp',
      company: 'Hunter Land',
      city: 'TP. Hồ Chí Minh',
      specialty: 'Căn hộ trung tâm, nhà phố thương mại',
      experience: '6 năm kinh nghiệm',
      listings: 42,
      phone: '090 123 4567',
      email: 'minhanh@hunterland.vn',
      avatarUrl:
        'https://images.unsplash.com/photo-1494790108377-be9c29b29330?auto=format&fit=crop&w=420&q=80',
    },
    {
      name: 'Trần Quốc Huy',
      role: 'Tư vấn đầu tư dự án',
      company: 'Gamuda Land',
      city: 'Hà Nội',
      specialty: 'Dự án đô thị, căn hộ bàn giao mới',
      experience: '8 năm kinh nghiệm',
      listings: 35,
      phone: '091 234 5678',
      email: 'quochuy@gamudaland.vn',
      avatarUrl:
        'https://images.unsplash.com/photo-1560250097-0b93528c311a?auto=format&fit=crop&w=420&q=80',
    },
    {
      name: 'Lê Hoàng Phúc',
      role: 'Môi giới đất nền và nhà phố',
      company: 'Kim Oanh Group',
      city: 'Bình Dương',
      specialty: 'Đất nền pháp lý rõ, nhà phố khu công nghiệp',
      experience: '5 năm kinh nghiệm',
      listings: 28,
      phone: '092 345 6789',
      email: 'hoangphuc@kimoanhgroup.vn',
      avatarUrl:
        'https://images.unsplash.com/photo-1519085360753-af0119f7cbe7?auto=format&fit=crop&w=420&q=80',
    },
    {
      name: 'Phạm Thanh Trúc',
      role: 'Chuyên viên cho thuê',
      company: 'SP Home',
      city: 'Đà Nẵng',
      specialty: 'Căn hộ dịch vụ, mặt bằng kinh doanh',
      experience: '4 năm kinh nghiệm',
      listings: 19,
      phone: '093 456 7890',
      email: 'thanhtruc@sphome.vn',
      avatarUrl:
        'https://images.unsplash.com/photo-1580489944761-15a19d654956?auto=format&fit=crop&w=420&q=80',
    },
  ];

  readonly businesses: BusinessProfile[] = [
    {
      name: 'Gamuda Land',
      type: 'Chủ đầu tư',
      city: 'Hà Nội',
      description: 'Phát triển khu đô thị và dự án căn hộ quy mô lớn.',
      projects: 12,
      agents: 46,
      logoUrl: '/logo/businesses/gamuda-land.png',
      websiteUrl: 'https://gamudaland.com.vn/',
    },
    {
      name: 'Phú Đông SkyOne',
      type: 'Đơn vị phát triển dự án',
      city: 'Bình Dương',
      description: 'Căn hộ đô thị vệ tinh dành cho nhu cầu ở thật.',
      projects: 7,
      agents: 24,
      logoUrl: '/logo/businesses/phu-dong-sky-one.png',
      websiteUrl: 'https://skyone.phudonggroup.com/',
    },
    {
      name: 'Cát Tường Group',
      type: 'Chủ đầu tư',
      city: 'Đồng Nai',
      description: 'Dự án khu dân cư, đất nền và nhà phố thương mại.',
      projects: 18,
      agents: 58,
      logoUrl: '/logo/businesses/cat-tuong-group.png',
      websiteUrl: 'https://cattuonggroup.vn/',
    },
    {
      name: 'Hunter Land',
      type: 'Sàn giao dịch',
      city: 'TP. Hồ Chí Minh',
      description: 'Phân phối căn hộ, nhà phố và dự án đầu tư tại phía Nam.',
      projects: 15,
      agents: 39,
      logoUrl: '/logo/businesses/hunter-land.png',
      websiteUrl: 'https://hunterland.com.vn/',
    },
    {
      name: 'Kim Oanh Group',
      type: 'Tập đoàn bất động sản',
      city: 'Bình Dương',
      description: 'Hệ sinh thái phát triển, phân phối và quản lý bất động sản.',
      projects: 22,
      agents: 64,
      logoUrl: '/logo/businesses/kim-oanh-group.jpg',
      websiteUrl: 'https://kimoanhgroup.vn/',
    },
    {
      name: 'SP Home',
      type: 'Sàn môi giới',
      city: 'Đà Nẵng',
      description: 'Tư vấn giao dịch căn hộ, nhà ở và mặt bằng thương mại.',
      projects: 9,
      agents: 21,
      logoUrl: '/logo/businesses/sp-home.jpg',
    },
  ];

  setActiveTab(tab: DirectoryTab): void {
    this.activeTab = tab;
  }

  selectCity(city: string): void {
    this.selectedCity = city;
  }

  submitSearch(): void {
    this.searchQuery = this.searchQuery.trim();
  }

  get filteredBrokers(): BrokerProfile[] {
    const keyword = this.normalize(this.searchQuery);

    return this.brokers.filter((broker) => {
      const matchesCity =
        this.selectedCity === this.allCitiesLabel || broker.city === this.selectedCity;
      const searchable = this.normalize(
        [broker.name, broker.role, broker.company, broker.city, broker.specialty].join(' '),
      );

      return matchesCity && (!keyword || searchable.includes(keyword));
    });
  }

  get filteredBusinesses(): BusinessProfile[] {
    const keyword = this.normalize(this.searchQuery);

    return this.businesses.filter((business) => {
      const matchesCity =
        this.selectedCity === this.allCitiesLabel || business.city === this.selectedCity;
      const searchable = this.normalize(
        [business.name, business.type, business.city, business.description].join(' '),
      );

      return matchesCity && (!keyword || searchable.includes(keyword));
    });
  }

  get visibleCount(): number {
    return this.activeTab === 'brokers'
      ? this.filteredBrokers.length
      : this.filteredBusinesses.length;
  }

  private normalize(value: string): string {
    return value
      .toLowerCase()
      .normalize('NFD')
      .replace(/[\u0300-\u036f]/g, '');
  }
}
