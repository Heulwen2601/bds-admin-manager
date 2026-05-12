import { Component, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { finalize, timeout } from 'rxjs';
import { AuthService } from '../../../core/services/auth.service';
import { RegisterRequest } from '../../../models';
import { Home } from '../../home/home';

@Component({
  selector: 'app-register',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, RouterLink, Home],
  templateUrl: './register.html',
  styleUrl: './register.scss'
})
export class RegisterComponent {
  registerForm: FormGroup;
  isLoading = false;
  errorMessage = '';

  private fb = inject(FormBuilder);
  private authService = inject(AuthService);
  private router = inject(Router);

  constructor() {
    this.registerForm = this.fb.group({
      fullName: ['', Validators.required],
      email: ['', [Validators.required, Validators.email]],
      password: ['', [Validators.required, Validators.minLength(6)]],
      confirmPassword: ['', Validators.required],
      phone: ['']
    }, { validators: this.passwordMatchValidator });
  }

  passwordMatchValidator(form: FormGroup) {
    const password = form.get('password');
    const confirmPassword = form.get('confirmPassword');

    if (password && confirmPassword && password.value !== confirmPassword.value) {
      confirmPassword.setErrors({ mustMatch: true });
      return { mustMatch: true };
    }

    return null;
  }

  onSubmit() {
    if (this.registerForm.valid) {
      this.isLoading = true;
      this.errorMessage = '';

      const formValue = this.registerForm.value;
      const phone = formValue.phone?.trim();
      const request: RegisterRequest = {
        fullName: formValue.fullName.trim(),
        email: formValue.email.trim(),
        password: formValue.password,
        confirmPassword: formValue.confirmPassword,
        ...(phone ? { phone } : {})
      };

      this.authService.register(request).pipe(
        timeout({ first: 15000 }),
        finalize(() => {
          this.isLoading = false;
        })
      ).subscribe({
        next: (response) => {
          if (response.success) {
            this.router.navigate(['/auth/login'], {
              queryParams: { message: 'Đăng ký thành công. Vui lòng đăng nhập.' }
            });
          } else {
            this.errorMessage = response.message || 'Đăng ký thất bại';
          }
        },
        error: (error) => {
          this.errorMessage = this.getRegisterError(error);
        }
      });
    } else {
      this.registerForm.markAllAsTouched();
    }
  }

  private getRegisterError(error: any): string {
    if (error?.name === 'TimeoutError') {
      return 'Hết thời gian chờ máy chủ. Kiểm tra API đang chạy và địa chỉ URL.';
    }

    if (error?.status === 0) {
      return 'Không kết nối được máy chủ. Kiểm tra URL API, CORS hoặc cài đặt HTTP/HTTPS.';
    }

    return error?.error?.message || error?.error?.title || 'Đăng ký thất bại. Vui lòng thử lại.';
  }

  closeAuth() {
    this.router.navigate(['/']);
  }
}
