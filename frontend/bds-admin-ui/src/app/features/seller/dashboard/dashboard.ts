import { CommonModule } from '@angular/common';
import { ChangeDetectorRef, Component, OnInit } from '@angular/core';
import { RouterLink } from '@angular/router';
import { forkJoin } from 'rxjs';
import { SellerApiService } from '../../../core/services/seller-api.service';
import { Lead, Property, SellerDashboard, SellerProfile } from '../../../models';

@Component({
  selector: 'app-seller-dashboard',
  standalone: true,
  imports: [CommonModule, RouterLink],
  templateUrl: './dashboard.html',
  styleUrl: './dashboard.scss',
})
export class SellerDashboardComponent implements OnInit {
  loading = true;
  errorMessage = '';

  dashboard: SellerDashboard | null = null;
  properties: Property[] = [];
  leads: Lead[] = [];
  profile: SellerProfile | null = null;

  constructor(
    private sellerApi: SellerApiService,
    private cdr: ChangeDetectorRef,
  ) {}

  ngOnInit(): void {
    this.loadDashboard();
  }

  loadDashboard(): void {
    this.loading = true;
    this.errorMessage = '';

    forkJoin({
      dashboard: this.sellerApi.getDashboard(),
      properties: this.sellerApi.getProperties(),
      leads: this.sellerApi.getLeads(),
      profile: this.sellerApi.getProfile(),
    }).subscribe({
      next: ({ dashboard, properties, leads, profile }) => {
        this.dashboard = dashboard.data;
        this.properties = properties.data ?? [];
        this.leads = leads.data ?? [];
        this.profile = profile.data;
        this.loading = false;
        this.cdr.detectChanges();
      },
      error: () => {
        this.errorMessage = 'Không thể tải dữ liệu tổng quan. Vui lòng thử lại.';
        this.loading = false;
        this.cdr.detectChanges();
      },
    });
  }

  get totalProperties(): number {
    return this.dashboard?.properties ?? this.properties.length;
  }

  get totalLeads(): number {
    return this.dashboard?.leads ?? this.leads.length;
  }

  get publishedProperties(): number {
    return this.countByStatus('Published');
  }

  get pendingProperties(): number {
    return this.dashboard?.pendingProperties ?? this.countByStatus('Pending');
  }

  get draftProperties(): number {
    return this.countByStatus('Draft');
  }

  get rejectedProperties(): number {
    return this.countByStatus('Rejected');
  }

  get latestProperties(): Property[] {
    return [...this.properties]
      .sort(
        (a, b) =>
          new Date(b.updatedAt || b.createdAt).getTime() -
          new Date(a.updatedAt || a.createdAt).getTime(),
      )
      .slice(0, 4);
  }

  get latestLeads(): Lead[] {
    return [...this.leads]
      .sort((a, b) => new Date(b.createdAt).getTime() - new Date(a.createdAt).getTime())
      .slice(0, 4);
  }

  get profileCompleteness(): number {
    const fields = [
      this.profile?.contactName,
      this.profile?.phone,
      this.profile?.sellerType,
      this.profile?.address,
      this.profile?.companyName || this.profile?.taxCode,
    ];
    const completed = fields.filter((value) => !!String(value ?? '').trim()).length;
    return Math.round((completed / fields.length) * 100);
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

  statusLabel(status?: string): string {
    switch (status) {
      case 'Published':
        return 'Đang hiển thị';
      case 'Pending':
        return 'Chờ duyệt';
      case 'Rejected':
        return 'Bị từ chối';
      case 'Draft':
        return 'Bản nháp';
      default:
        return status || 'Không rõ';
    }
  }

  leadName(lead: Lead): string {
    return lead.fullName || lead.guestName || 'Khách hàng';
  }

  leadPhone(lead: Lead): string {
    return lead.phone || lead.guestPhone || 'Chưa có số điện thoại';
  }

  private countByStatus(status: string): number {
    return this.properties.filter((property) => property.status === status).length;
  }
}
