import { CommonModule } from '@angular/common';
import { Component, DestroyRef, inject } from '@angular/core';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { AuthService } from '../../../core/services/auth.service';
import { SellerApiService } from '../../../core/services/seller-api.service';
import { BecomeSellerRequest, SellerType } from '../../../models';

@Component({
  selector: 'app-become-seller',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './become-seller.html',
  styleUrl: './become-seller.scss',
})
export class BecomeSellerComponent {
  private readonly fb = inject(FormBuilder);
  private readonly sellerApi = inject(SellerApiService);
  private readonly authService = inject(AuthService);
  private readonly router = inject(Router);
  private readonly destroyRef = inject(DestroyRef);

  readonly sellerTypeOptions: Array<{
    value: SellerType;
    title: string;
    description: string;
  }> = [
    {
      value: 'Broker',
      title: 'Môi giới',
      description: 'Tư vấn và môi giới giao dịch bất động sản cho khách hàng.',
    },
    {
      value: 'CompanyRepresentative',
      title: 'Đại diện công ty',
      description: 'Đăng tin thay mặt chủ đầu tư, sàn giao dịch hoặc doanh nghiệp.',
    },
    {
      value: 'Owner',
      title: 'Chủ nhà / người bán lẻ',
      description: 'Đăng bán hoặc cho thuê bất động sản thuộc sở hữu của mình.',
    },
  ];

  readonly sellerForm = this.fb.group({
    sellerType: ['Broker' as SellerType, Validators.required],
    companyName: ['', [Validators.maxLength(150)]],
    contactName: ['', [Validators.required, Validators.maxLength(100)]],
    phone: [
      '',
      [Validators.required, Validators.maxLength(20), Validators.pattern(/^[0-9+()\s.-]{8,20}$/)],
    ],
    address: ['', [Validators.maxLength(300)]],
    taxCode: ['', [Validators.maxLength(50)]],
  });

  isSubmitting = false;
  errorMessage = '';
  successMessage = '';

  constructor() {
    const user = this.authService.getUser();
    this.sellerForm.patchValue({
      contactName: user?.fullName ?? '',
      phone: user?.phone ?? '',
    });

    this.applyCompanyValidators(this.sellerType);
    this.sellerForm.controls.sellerType.valueChanges
      .pipe(takeUntilDestroyed(this.destroyRef))
      .subscribe((sellerType) => this.applyCompanyValidators(sellerType ?? 'Broker'));
  }

  get sellerType(): SellerType {
    return this.sellerForm.controls.sellerType.value ?? 'Broker';
  }

  get requiresCompany(): boolean {
    return this.requiresCompanyFor(this.sellerType);
  }

  submit(): void {
    if (this.sellerForm.invalid) {
      this.sellerForm.markAllAsTouched();
      return;
    }

    this.isSubmitting = true;
    this.errorMessage = '';
    this.successMessage = '';

    this.sellerApi
      .becomeSeller(this.toRequest())
      .pipe(takeUntilDestroyed(this.destroyRef))
      .subscribe({
        next: (response) => {
          if (!response.success || !response.data) {
            this.errorMessage = response.message || 'Không thể tạo hồ sơ người bán.';
            this.isSubmitting = false;
            return;
          }

          this.authService.setToken(response.data.auth.token);
          const currentUser = this.authService.getUser();
          if (currentUser) {
            this.authService.setUser({ ...currentUser, role: response.data.auth.role });
          }

          this.isSubmitting = false;
          this.successMessage = 'Hồ sơ người bán đã được tạo.';
          this.router.navigate(['/seller/dashboard']);
        },
        error: (error) => {
          this.errorMessage =
            error.error?.message ||
            error.error?.title ||
            'Không thể tạo hồ sơ người bán. Vui lòng kiểm tra thông tin.';
          this.isSubmitting = false;
        },
      });
  }

  isInvalid(controlName: keyof typeof this.sellerForm.controls): boolean {
    const control = this.sellerForm.controls[controlName];
    return control.invalid && (control.dirty || control.touched);
  }

  private applyCompanyValidators(sellerType: SellerType): void {
    const companyControl = this.sellerForm.controls.companyName;
    companyControl.setValidators(
      this.requiresCompanyFor(sellerType)
        ? [Validators.required, Validators.maxLength(150)]
        : [Validators.maxLength(150)],
    );
    companyControl.updateValueAndValidity({ emitEvent: false });
  }

  private requiresCompanyFor(sellerType: SellerType): boolean {
    return sellerType === 'CompanyRepresentative';
  }

  private toRequest(): BecomeSellerRequest {
    const value = this.sellerForm.getRawValue();
    const sellerType = value.sellerType ?? 'Broker';

    return {
      sellerType,
      companyName: this.requiresCompanyFor(sellerType)
        ? this.normalize(value.companyName)
        : undefined,
      contactName: this.normalize(value.contactName) ?? '',
      phone: this.normalize(value.phone) ?? '',
      address: this.normalize(value.address),
      taxCode: this.normalize(value.taxCode),
    };
  }

  private normalize(value: string | null | undefined): string | undefined {
    const trimmed = value?.trim();
    return trimmed ? trimmed : undefined;
  }
}
