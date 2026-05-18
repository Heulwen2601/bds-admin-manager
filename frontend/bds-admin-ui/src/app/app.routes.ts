import { Routes } from '@angular/router';
import { AuthGuard } from './core/guards/auth.guard';
import { RoleGuard } from './core/guards/role.guard';

export const routes: Routes = [
  // Public routes
  {
    path: '',
    loadComponent: () =>
      import('./layouts/public-layout/public-layout').then((m) => m.PublicLayoutComponent),
    children: [
      {
        path: '',
        loadComponent: () => import('./features/home/home').then((m) => m.Home),
      },
      {
        path: 'properties',
        loadComponent: () =>
          import('./features/properties/property-list/property-list').then(
            (m) => m.PropertyListComponent,
          ),
      },
      {
        path: 'for-sale',
        loadComponent: () =>
          import('./features/properties/property-list/property-list').then(
            (m) => m.PropertyListComponent,
          ),
        data: { categoryGroup: 'for-sale', title: 'Nhà đất bán' },
      },
      {
        path: 'for-rent',
        loadComponent: () =>
          import('./features/properties/property-list/property-list').then(
            (m) => m.PropertyListComponent,
          ),
        data: { categoryGroup: 'for-rent', title: 'Nhà đất cho thuê' },
      },
      {
        path: 'projects',
        loadComponent: () =>
          import('./features/properties/property-list/property-list').then(
            (m) => m.PropertyListComponent,
          ),
        data: { categoryGroup: 'project-properties', title: 'Dự án bất động sản' },
      },
      {
        path: 'directory',
        loadComponent: () =>
          import('./features/directory/directory').then((m) => m.DirectoryComponent),
      },
      {
        path: 'properties/:id',
        loadComponent: () =>
          import('./features/properties/property-detail/property-detail').then(
            (m) => m.PropertyDetailComponent,
          ),
      },
      {
        path: 'search',
        loadComponent: () =>
          import('./features/properties/property-search/property-search').then(
            (m) => m.PropertySearchComponent,
          ),
      },
    ],
  },

  // Auth routes
  {
    path: 'auth',
    children: [
      {
        path: 'login',
        loadComponent: () => import('./features/auth/login/login').then((m) => m.LoginComponent),
      },
      {
        path: 'register',
        loadComponent: () =>
          import('./features/auth/register/register').then((m) => m.RegisterComponent),
      },
    ],
  },

  // User routes (authenticated)
  {
    path: 'user',
    canActivate: [AuthGuard],
    children: [
      {
        path: 'become-seller',
        loadComponent: () =>
          import('./features/user/become-seller/become-seller').then(
            (m) => m.BecomeSellerComponent,
          ),
      },
    ],
  },

  // Seller routes
  {
    path: 'seller',
    canActivate: [AuthGuard, RoleGuard],
    data: { roles: ['Seller'] },
    loadComponent: () =>
      import('./layouts/seller-layout/seller-layout').then((m) => m.SellerLayoutComponent),
    children: [
      {
        path: 'dashboard',
        loadComponent: () =>
          import('./features/seller/dashboard/dashboard').then((m) => m.SellerDashboardComponent),
      },
      {
        path: 'properties',
        loadComponent: () =>
          import('./features/seller/property-list/property-list').then(
            (m) => m.SellerPropertyListComponent,
          ),
      },
      {
        path: 'properties/create',
        loadComponent: () =>
          import('./features/seller/property-form/property-form').then(
            (m) => m.PropertyFormComponent,
          ),
      },
      {
        path: 'properties/:id/edit',
        loadComponent: () =>
          import('./features/seller/property-form/property-form').then(
            (m) => m.PropertyFormComponent,
          ),
      },
      {
        path: 'leads',
        loadComponent: () =>
          import('./features/seller/leads/leads').then((m) => m.SellerLeadsComponent),
      },
      {
        path: 'profile',
        loadComponent: () =>
          import('./features/seller/profile/profile').then((m) => m.SellerProfileComponent),
      },
    ],
  },

  // Admin routes
  {
    path: 'admin',
    canActivate: [AuthGuard, RoleGuard],
    data: { roles: ['Admin'] },
    loadComponent: () =>
      import('./layouts/admin-layout/admin-layout').then((m) => m.AdminLayoutComponent),
    children: [
      {
        path: 'dashboard',
        loadComponent: () =>
          import('./features/admin/dashboard/dashboard').then((m) => m.AdminDashboardComponent),
      },
      {
        path: 'properties',
        loadComponent: () =>
          import('./features/admin/property-management/property-management').then(
            (m) => m.AdminPropertyManagementComponent,
          ),
      },
      {
        path: 'categories',
        loadComponent: () =>
          import('./features/admin/category-management/category-management').then(
            (m) => m.AdminCategoryManagementComponent,
          ),
      },
      {
        path: 'consultant-performance',
        loadComponent: () =>
          import('./features/admin/consultant-performance/consultant-performance').then(
            (m) => m.ConsultantPerformanceComponent,
          ),
      },
    ],
  },

  // Consultant routes
  {
    path: 'consultant',
    canActivate: [AuthGuard, RoleGuard],
    data: { roles: ['Consultant'] },
    loadComponent: () =>
      import('./layouts/consultant-layout/consultant-layout').then(
        (m) => m.ConsultantLayoutComponent,
      ),
    children: [
      {
        path: 'conversations',
        loadComponent: () =>
          import('./features/consultant/conversation-list/conversation-list').then(
            (m) => m.ConsultantConversationListComponent,
          ),
      },
      {
        path: 'conversations/:id',
        loadComponent: () =>
          import('./features/consultant/chat-detail/chat-detail').then(
            (m) => m.ChatDetailComponent,
          ),
      },
    ],
  },

  // Notifications
  {
    path: 'notifications',
    canActivate: [AuthGuard],
    loadComponent: () =>
      import('./features/notifications/notification-list/notification-list').then(
        (m) => m.NotificationListComponent,
      ),
  },

  // Chat
  {
    path: 'chat',
    canActivate: [AuthGuard],
    loadComponent: () =>
      import('./features/chat/chat-widget/chat-widget').then((m) => m.ChatWidgetComponent),
  },

  // Wildcard route
  { path: '**', redirectTo: '' },
];
