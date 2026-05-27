import { CommonModule } from '@angular/common';
import { ChangeDetectorRef, Component, OnInit, inject } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { RouterLink } from '@angular/router';
import { forkJoin } from 'rxjs';
import { SellerApiService } from '../../../core/services/seller-api.service';
import { Lead, Property } from '../../../models';

type LeadFilter = 'all' | 'today' | 'week' | 'with-email' | 'with-message';

@Component({
  selector: 'app-seller-leads',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterLink],
  templateUrl: './leads.html',
  styleUrl: './leads.scss',
})
export class SellerLeadsComponent implements OnInit {
  private readonly sellerApi = inject(SellerApiService);
  private readonly cdr = inject(ChangeDetectorRef);

  loading = true;
  acceptingPolicy = false;
  hasAcceptedPolicy = false;
  policyAcceptedAt?: string;
  errorMessage = '';
  keyword = '';
  activeFilter: LeadFilter = 'all';

  leads: Lead[] = [];
  properties: Property[] = [];

  readonly filters: Array<{ value: LeadFilter; label: string }> = [
    { value: 'all', label: 'Tất cả' },
    { value: 'today', label: 'Hôm nay' },
    { value: 'week', label: '7 ngày' },
    { value: 'with-email', label: 'Có email' },
    { value: 'with-message', label: 'Có lời nhắn' },
  ];

  ngOnInit(): void {
    this.loadConsent();
  }

  loadConsent(): void {
    this.loading = true;
    this.errorMessage = '';

    this.sellerApi.getCustomerDataPolicyConsent().subscribe({
      next: (response) => {
        this.hasAcceptedPolicy = !!response.data?.hasAccepted;
        this.policyAcceptedAt = response.data?.acceptedAt;
        if (this.hasAcceptedPolicy) {
          this.loadData();
          return;
        }

        this.loading = false;
        this.cdr.detectChanges();
      },
      error: (error) => {
        this.errorMessage =
          error.error?.message || error.error?.title || 'Không thể kiểm tra xác nhận bảo mật.';
        this.loading = false;
        this.cdr.detectChanges();
      },
    });
  }

  acceptPolicy(): void {
    this.acceptingPolicy = true;
    this.errorMessage = '';

    this.sellerApi.acceptCustomerDataPolicy().subscribe({
      next: (response) => {
        this.hasAcceptedPolicy = !!response.data?.hasAccepted;
        this.policyAcceptedAt = response.data?.acceptedAt;
        this.acceptingPolicy = false;
        this.loadData();
      },
      error: (error) => {
        this.errorMessage =
          error.error?.message || error.error?.title || 'Không thể lưu xác nhận bảo mật.';
        this.acceptingPolicy = false;
        this.cdr.detectChanges();
      },
    });
  }

  loadData(): void {
    if (!this.hasAcceptedPolicy) {
      this.loadConsent();
      return;
    }

    this.loading = true;
    this.errorMessage = '';

    forkJoin({
      leads: this.sellerApi.getLeads(),
      properties: this.sellerApi.getProperties(),
    }).subscribe({
      next: ({ leads, properties }) => {
        this.leads = leads.data ?? [];
        this.properties = properties.data ?? [];
        this.loading = false;
        this.cdr.detectChanges();
      },
      error: (error) => {
        this.errorMessage =
          error.error?.message || error.error?.title || 'Không thể tải danh sách khách hàng.';
        this.loading = false;
        this.cdr.detectChanges();
      },
    });
  }

  get filteredLeads(): Lead[] {
    const keyword = this.keyword.trim().toLowerCase();

    return this.leads.filter((lead) => {
      if (!this.matchesFilter(lead)) {
        return false;
      }

      if (!keyword) {
        return true;
      }

      const haystack = [
        this.leadName(lead),
        this.leadPhone(lead),
        this.leadEmail(lead),
        lead.message,
        this.propertyTitle(lead),
        this.propertyListingCode(lead),
      ]
        .filter(Boolean)
        .join(' ')
        .toLowerCase();

      return haystack.includes(keyword);
    });
  }

  get totalLeads(): number {
    return this.leads.length;
  }

  get todayLeads(): number {
    const today = new Date();
    return this.leads.filter((lead) => this.isSameDate(new Date(lead.createdAt), today)).length;
  }

  get weekLeads(): number {
    const cutoff = this.daysAgo(7);
    return this.leads.filter((lead) => new Date(lead.createdAt) >= cutoff).length;
  }

  get contactedProperties(): number {
    return new Set(this.leads.map((lead) => lead.propertyId)).size;
  }

  get unreadLeads(): number {
    return this.leads.filter((lead) => !lead.isRead).length;
  }

  filterCount(filter: LeadFilter): number {
    return this.leads.filter((lead) => this.matchesFilter(lead, filter)).length;
  }

  setFilter(filter: LeadFilter): void {
    this.activeFilter = filter;
  }

  clearSearch(): void {
    this.keyword = '';
  }

  leadName(lead: Lead): string {
    return lead.fullName || lead.guestName || 'Khách hàng';
  }

  leadPhone(lead: Lead): string {
    return lead.phone || lead.guestPhone || 'Chưa có số điện thoại';
  }

  leadEmail(lead: Lead): string {
    return lead.email || lead.guestEmail || '';
  }

  propertyTitle(lead: Lead): string {
    return (
      lead.propertyTitle || this.propertyById(lead.propertyId)?.title || 'Tin đăng không xác định'
    );
  }

  propertyListingCode(lead: Lead): string {
    return lead.propertyListingCode || this.propertyById(lead.propertyId)?.listingCode || '';
  }

  propertyStatus(lead: Lead): string {
    return lead.propertyStatus || this.propertyById(lead.propertyId)?.status || '';
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

  formatDate(value?: string): string {
    if (!value) {
      return 'Chưa cập nhật';
    }

    return new Intl.DateTimeFormat('vi-VN', {
      day: '2-digit',
      month: '2-digit',
      year: 'numeric',
      hour: '2-digit',
      minute: '2-digit',
    }).format(new Date(value));
  }

  phoneHref(lead: Lead): string {
    const phone = this.leadPhone(lead);
    return phone ? `tel:${phone.replace(/\s/g, '')}` : '';
  }

  mailHref(lead: Lead): string {
    const email = this.leadEmail(lead);
    return email ? `mailto:${email}` : '';
  }

  markRead(lead: Lead): void {
    if (lead.isRead) {
      return;
    }

    this.sellerApi.markLeadRead(lead.id).subscribe({
      next: (response) => {
        if (response.data) {
          this.leads = this.leads.map((item) => (item.id === lead.id ? response.data! : item));
        }
        this.cdr.detectChanges();
      },
      error: (error) => {
        this.errorMessage =
          error.error?.message || error.error?.title || 'Không thể đánh dấu khách hàng đã đọc.';
        this.cdr.detectChanges();
      },
    });
  }

  private matchesFilter(lead: Lead, filter = this.activeFilter): boolean {
    switch (filter) {
      case 'today':
        return this.isSameDate(new Date(lead.createdAt), new Date());
      case 'week':
        return new Date(lead.createdAt) >= this.daysAgo(7);
      case 'with-email':
        return !!this.leadEmail(lead);
      case 'with-message':
        return !!lead.message?.trim();
      case 'all':
      default:
        return true;
    }
  }

  private propertyById(propertyId: string): Property | undefined {
    return this.properties.find((property) => property.id === propertyId);
  }

  private daysAgo(days: number): Date {
    const date = new Date();
    date.setDate(date.getDate() - days);
    return date;
  }

  private isSameDate(first: Date, second: Date): boolean {
    return (
      first.getFullYear() === second.getFullYear() &&
      first.getMonth() === second.getMonth() &&
      first.getDate() === second.getDate()
    );
  }
}
