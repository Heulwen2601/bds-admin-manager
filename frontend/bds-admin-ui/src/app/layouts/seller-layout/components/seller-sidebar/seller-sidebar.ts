import { CommonModule } from '@angular/common';
import { Component } from '@angular/core';
import { RouterLink, RouterLinkActive } from '@angular/router';

interface SellerNavItem {
  label: string;
  route: string;
  icon: string;
  exact?: boolean;
}

@Component({
  selector: 'app-seller-sidebar',
  standalone: true,
  imports: [CommonModule, RouterLink, RouterLinkActive],
  templateUrl: './seller-sidebar.html',
  styleUrl: './seller-sidebar.scss',
})
export class SellerSidebarComponent {
  collapsed = localStorage.getItem('seller_sidebar_collapsed') === 'true';

  primaryItems: SellerNavItem[] = [
    {
      label: 'Tổng quan',
      route: '/seller/dashboard',
      icon: 'overview',
      exact: true,
    },
    {
      label: 'Quản lý tin đăng',
      route: '/seller/properties',
      icon: 'list',
    },
    {
      label: 'Đăng tin mới',
      route: '/seller/properties/create',
      icon: 'plus',
      exact: true,
    },
    {
      label: 'Quản lý khách hàng',
      route: '/seller/leads',
      icon: 'users',
      exact: true,
    },
  ];
  secondaryItems: SellerNavItem[] = [
    {
      label: 'Cài đặt tài khoản',
      route: '/seller/profile',
      icon: 'settings',
      exact: true,
    },
  ];

  toggleCollapsed(): void {
    this.collapsed = !this.collapsed;
    localStorage.setItem('seller_sidebar_collapsed', String(this.collapsed));
  }
}

