import { Component, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { finalize, timeout } from 'rxjs';
import { HttpErrorResponse } from '@angular/common/http';
import { AuthService } from '../../../core/services/auth.service';
import { Home } from '../../home/home';

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, RouterLink, Home],
  templateUrl: './login.html',
  styleUrl: './login.scss',
})
export class LoginComponent {
  loginForm: FormGroup;
  isLoading = false;
  errorMessage = '';

  private fb = inject(FormBuilder);
  private authService = inject(AuthService);
  private router = inject(Router);

  constructor() {
    this.loginForm = this.fb.group({
      email: ['', [Validators.required, Validators.email]],
      password: ['', Validators.required],
    });
  }

  onSubmit() {
    if (this.loginForm.valid) {
      this.isLoading = true;
      this.errorMessage = '';

      const { email, password } = this.loginForm.value;

      this.authService
        .login(email, password)
        .pipe(
          timeout(15000),
          finalize(() => {
            if (this.isLoading) {
              this.isLoading = false;
            }
          }),
        )
        .subscribe({
          next: (response) => {
            if (response.success && response.data) {
              this.authService.setToken(response.data.token);

              this.authService
                .getCurrentUser()
                .pipe(
                  timeout(15000),
                  finalize(() => {
                    if (this.isLoading) {
                      this.isLoading = false;
                    }
                  }),
                )
                .subscribe({
                  next: (userResponse) => {
                    if (userResponse.success && userResponse.data) {
                      this.authService.setUser(userResponse.data);
                      this.authService.redirectAfterLogin();
                    } else {
                      this.errorMessage =
                        userResponse.message ||
                        'Không thể lấy thông tin người dùng sau khi đăng nhập.';
                    }
                  },
                  error: (error) => {
                    this.errorMessage = this.extractErrorMessage(
                      error,
                      'Không thể lấy thông tin người dùng sau khi đăng nhập.',
                    );
                  },
                });
            } else {
              this.errorMessage = response?.message || 'Email hoặc mật khẩu không đúng.';
            }
          },
          error: (error) => {
            this.errorMessage = this.extractErrorMessage(error, 'Email hoặc mật khẩu không đúng.');
          },
        });
    } else {
      this.loginForm.markAllAsTouched();
    }
  }

  private extractErrorMessage(error: unknown, defaultMessage: string): string {
    if (!error) {
      return defaultMessage;
    }

    if (typeof error === 'string') {
      return error;
    }

    if (error instanceof HttpErrorResponse) {
      if (error.error instanceof ErrorEvent) {
        return 'Không thể kết nối đến server. Vui lòng kiểm tra kết nối mạng hoặc backend.';
      }

      if (error.status === 0) {
        return 'Không thể kết nối tới backend. Vui lòng kiểm tra server hoặc mạng.';
      }

      if (error.status >= 500) {
        return 'Server gặp sự cố. Vui lòng thử lại sau.';
      }

      if (error.error && typeof error.error === 'object' && error.error !== null) {
        const errorObject = error.error as { message?: string; errors?: unknown };
        if (errorObject.message) {
          return errorObject.message;
        }
        if (Array.isArray(errorObject.errors)) {
          return errorObject.errors.join(', ');
        }
      }

      return error.message || defaultMessage;
    }

    const err = error as { error?: unknown; message?: string };

    if (err.error) {
      if (typeof err.error === 'string') {
        return err.error;
      }
      if (typeof err.error === 'object' && err.error !== null) {
        const errorObject = err.error as { message?: string; errors?: unknown };
        if (errorObject.message) {
          return errorObject.message;
        }
        if (Array.isArray(errorObject.errors)) {
          return errorObject.errors.join(', ');
        }
      }
    }

    return err.message || defaultMessage;
  }

  closeAuth() {
    this.router.navigate(['/']);
  }
}
