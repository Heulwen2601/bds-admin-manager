import { CommonModule } from '@angular/common';
import { ChangeDetectorRef, Component, DestroyRef, inject } from '@angular/core';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { ActivatedRoute } from '@angular/router';
import { AuthService } from '../../../core/services/auth.service';
import { SellerApiService } from '../../../core/services/seller-api.service';
import {
  SellerChangePasswordRequest,
  SellerProfile,
  SellerType,
  UpdateSellerProfileRequest,
} from '../../../models';

@Component({
  selector: 'app-seller-profile',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './profile.html',
  styleUrl: './profile.scss',
})
export class SellerProfileComponent {
  private readonly fb = inject(FormBuilder);
  private readonly sellerApi = inject(SellerApiService);
  private readonly authService = inject(AuthService);
  private readonly destroyRef = inject(DestroyRef);
  private readonly cdr = inject(ChangeDetectorRef);
  private readonly route = inject(ActivatedRoute);

  readonly sellerTypeOptions: Array<{ value: SellerType; label: string }> = [
    { value: 'Broker', label: 'Môi giới' },
    { value: 'CompanyRepresentative', label: 'Đại diện công ty' },
    { value: 'Owner', label: 'Chủ nhà / người bán lẻ' },
  ];

  readonly profileForm = this.fb.group({
    sellerType: ['Broker' as SellerType, Validators.required],
    contactName: ['', [Validators.required, Validators.maxLength(100)]],
    phone: [
      '',
      [Validators.required, Validators.maxLength(20), Validators.pattern(/^[0-9+()\s.-]{8,20}$/)],
    ],
    additionalPhone: ['', [Validators.maxLength(20), Validators.pattern(/^[0-9+()\s.-]{8,20}$/)]],
    companyName: ['', [Validators.maxLength(150)]],
    address: ['', [Validators.maxLength(300)]],
    taxCode: ['', [Validators.maxLength(50)]],
    invoiceBuyerName: ['', [Validators.maxLength(100)]],
    invoiceEmail: ['', [Validators.email, Validators.maxLength(150)]],
    invoiceCompanyName: ['', [Validators.maxLength(150)]],
    budgetUnitCode: ['', [Validators.maxLength(50)]],
    citizenId: ['', [Validators.maxLength(20)]],
    passportNumber: ['', [Validators.maxLength(30)]],
    invoiceAddress: ['', [Validators.maxLength(300)]],
  });

  readonly passwordForm = this.fb.group({
    currentPassword: ['', [Validators.required]],
    newPassword: [
      '',
      [
        Validators.required,
        Validators.minLength(8),
        Validators.pattern(/^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^A-Za-z0-9]).+$/),
      ],
    ],
    confirmPassword: ['', [Validators.required]],
  });

  profile: SellerProfile | null = null;
  activeTab: 'account' | 'settings' = 'account';
  loading = true;
  saving = false;
  changingPassword = false;
  editing = false;
  errorMessage = '';
  successMessage = '';
  showAdditionalPhone = false;
  private pendingPasswordScroll = false;
  collapsedSections: Record<'lock' | 'delete' | 'support', boolean> = {
    lock: true,
    delete: true,
    support: true,
  };

  constructor() {
    this.route.fragment.pipe(takeUntilDestroyed(this.destroyRef)).subscribe((fragment) => {
      if (fragment === 'password') {
        this.showPasswordSettings();
      }
    });

    this.loadProfile();
  }

  get sellerType(): SellerType {
    return this.profileForm.controls.sellerType.value ?? 'Broker';
  }

  get currentUser() {
    return this.authService.getUser();
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

  loadProfile(): void {
    this.loading = true;
    this.errorMessage = '';
    this.successMessage = '';

    this.sellerApi
      .getProfile()
      .pipe(takeUntilDestroyed(this.destroyRef))
      .subscribe({
        next: (response) => {
          this.profile = response.data;
          this.profileForm.reset({
            sellerType: response.data.sellerType,
            contactName: response.data.contactName,
            phone: response.data.phone,
            additionalPhone: response.data.additionalPhone ?? '',
            companyName: response.data.companyName ?? '',
            address: response.data.address ?? '',
            taxCode: response.data.taxCode ?? '',
            invoiceBuyerName: response.data.invoiceBuyerName ?? response.data.contactName,
            invoiceEmail: response.data.invoiceEmail ?? this.currentUser?.email ?? '',
            invoiceCompanyName: response.data.invoiceCompanyName ?? response.data.companyName ?? '',
            budgetUnitCode: response.data.budgetUnitCode ?? '',
            citizenId: response.data.citizenId ?? '',
            passportNumber: response.data.passportNumber ?? '',
            invoiceAddress: response.data.invoiceAddress ?? response.data.address ?? '',
          });
          this.loading = false;
          this.showAdditionalPhone = !!response.data.additionalPhone;
          this.cdr.detectChanges();
          this.scrollToPasswordIfNeeded();
        },
        error: () => {
          this.errorMessage = 'Không thể tải thông tin seller.';
          this.loading = false;
          this.cdr.detectChanges();
        },
      });
  }

  startEdit(): void {
    this.editing = true;
    this.successMessage = '';
    this.errorMessage = '';
  }

  selectTab(tab: 'account' | 'settings'): void {
    this.activeTab = tab;
    this.successMessage = '';
    this.errorMessage = '';
  }

  showPasswordSettings(): void {
    this.activeTab = 'settings';
    this.pendingPasswordScroll = true;
    this.successMessage = '';
    this.errorMessage = '';
    this.cdr.detectChanges();
    this.scrollToPasswordIfNeeded();
  }

  toggleSection(section: 'lock' | 'delete' | 'support'): void {
    this.collapsedSections[section] = !this.collapsedSections[section];
  }

  cancelEdit(): void {
    this.editing = false;
    if (this.profile) {
      this.profileForm.reset({
        sellerType: this.profile.sellerType,
        contactName: this.profile.contactName,
        phone: this.profile.phone,
        additionalPhone: this.profile.additionalPhone ?? '',
        companyName: this.profile.companyName ?? '',
        address: this.profile.address ?? '',
        taxCode: this.profile.taxCode ?? '',
        invoiceBuyerName: this.profile.invoiceBuyerName ?? this.profile.contactName,
        invoiceEmail: this.profile.invoiceEmail ?? this.currentUser?.email ?? '',
        invoiceCompanyName: this.profile.invoiceCompanyName ?? this.profile.companyName ?? '',
        budgetUnitCode: this.profile.budgetUnitCode ?? '',
        citizenId: this.profile.citizenId ?? '',
        passportNumber: this.profile.passportNumber ?? '',
        invoiceAddress: this.profile.invoiceAddress ?? this.profile.address ?? '',
      });
    }
  }

  addAdditionalPhone(): void {
    this.showAdditionalPhone = true;
    this.cdr.detectChanges();
  }

  removeAdditionalPhone(): void {
    this.profileForm.controls.additionalPhone.reset('');
    this.showAdditionalPhone = false;
  }

  saveProfile(): void {
    if (this.profileForm.invalid) {
      this.profileForm.markAllAsTouched();
      return;
    }

    this.saving = true;
    this.errorMessage = '';
    this.successMessage = '';

    this.sellerApi
      .updateProfile(this.toRequest())
      .pipe(takeUntilDestroyed(this.destroyRef))
      .subscribe({
        next: (response) => {
          this.profile = response.data;
          this.saving = false;
          this.editing = false;
          this.successMessage = 'Thông tin seller đã được cập nhật.';
          this.cdr.detectChanges();
        },
        error: (error) => {
          this.errorMessage =
            error.error?.message || error.error?.title || 'Không thể cập nhật thông tin seller.';
          this.saving = false;
          this.cdr.detectChanges();
        },
      });
  }

  changePassword(): void {
    if (this.passwordForm.invalid || !this.passwordsMatch()) {
      this.passwordForm.markAllAsTouched();
      return;
    }

    this.changingPassword = true;
    this.errorMessage = '';
    this.successMessage = '';

    this.sellerApi
      .changePassword(this.toPasswordRequest())
      .pipe(takeUntilDestroyed(this.destroyRef))
      .subscribe({
        next: () => {
          this.passwordForm.reset();
          this.changingPassword = false;
          this.successMessage = 'Mật khẩu đã được thay đổi.';
          this.cdr.detectChanges();
        },
        error: (error) => {
          this.errorMessage =
            error.error?.message || error.error?.title || 'Không thể đổi mật khẩu.';
          this.changingPassword = false;
          this.cdr.detectChanges();
        },
      });
  }

  passwordsMatch(): boolean {
    const value = this.passwordForm.getRawValue();
    return (
      !value.newPassword || !value.confirmPassword || value.newPassword === value.confirmPassword
    );
  }

  isInvalid(controlName: keyof typeof this.profileForm.controls): boolean {
    const control = this.profileForm.controls[controlName];
    return control.invalid && (control.dirty || control.touched);
  }

  private toRequest(): UpdateSellerProfileRequest {
    const value = this.profileForm.getRawValue();
    return {
      sellerType: value.sellerType ?? 'Broker',
      contactName: this.normalize(value.contactName) ?? '',
      phone: this.normalize(value.phone) ?? '',
      additionalPhone: this.normalize(value.additionalPhone),
      companyName: this.normalize(value.companyName),
      address: this.normalize(value.address),
      taxCode: this.normalize(value.taxCode),
      invoiceBuyerName: this.normalize(value.invoiceBuyerName),
      invoiceEmail: this.normalize(value.invoiceEmail),
      invoiceCompanyName: this.normalize(value.invoiceCompanyName),
      budgetUnitCode: this.normalize(value.budgetUnitCode),
      citizenId: this.normalize(value.citizenId),
      passportNumber: this.normalize(value.passportNumber),
      invoiceAddress: this.normalize(value.invoiceAddress),
    };
  }

  private toPasswordRequest(): SellerChangePasswordRequest {
    const value = this.passwordForm.getRawValue();
    return {
      currentPassword: value.currentPassword ?? '',
      newPassword: value.newPassword ?? '',
      confirmPassword: value.confirmPassword ?? '',
    };
  }

  private normalize(value: string | null | undefined): string | undefined {
    const trimmed = value?.trim();
    return trimmed ? trimmed : undefined;
  }

  private scrollToPasswordIfNeeded(): void {
    if (!this.pendingPasswordScroll || this.loading) {
      return;
    }

    this.pendingPasswordScroll = false;
    setTimeout(() => {
      document.getElementById('password')?.scrollIntoView({ behavior: 'smooth', block: 'start' });
      this.passwordForm.controls.currentPassword.markAsUntouched();
    });
  }
}
