import { Injectable } from '@angular/core';
import { BehaviorSubject, Observable } from 'rxjs';
import { Router } from '@angular/router';
import { AuthApiService } from './auth-api.service';
import { User, AuthState } from '../../models';

@Injectable({
  providedIn: 'root',
})
export class AuthService {
  private readonly TOKEN_KEY = 'auth_token';
  private readonly USER_KEY = 'auth_user';

  private authStateSubject = new BehaviorSubject<AuthState>({
    user: null,
    token: null,
    isAuthenticated: false,
  });

  public authState$ = this.authStateSubject.asObservable();

  constructor(
    private authApi: AuthApiService,
    private router: Router,
  ) {
    this.loadStoredAuth();
  }

  login(email: string, password: string): Observable<any> {
    return this.authApi.login({ username: email, password });
  }

  register(userData: any): Observable<any> {
    return this.authApi.register(userData);
  }

  logout(): void {
    this.clearAuth();
    this.router.navigate(['/auth/login']);
    this.authApi.logout().subscribe({ error: () => undefined });
  }

  clearSession(): void {
    this.clearAuth();
  }

  setAuth(token: string, user: User): void {
    localStorage.setItem(this.TOKEN_KEY, token);
    localStorage.setItem(this.USER_KEY, JSON.stringify(user));

    this.authStateSubject.next({
      user,
      token,
      isAuthenticated: true,
    });
  }

  setToken(token: string): void {
    localStorage.setItem(this.TOKEN_KEY, token);

    this.authStateSubject.next({
      ...this.authStateSubject.value,
      token,
      isAuthenticated: true,
    });
  }

  setUser(user: User): void {
    localStorage.setItem(this.USER_KEY, JSON.stringify(user));

    this.authStateSubject.next({
      ...this.authStateSubject.value,
      user,
      isAuthenticated: true,
    });
  }

  private clearAuth(): void {
    localStorage.removeItem(this.TOKEN_KEY);
    localStorage.removeItem(this.USER_KEY);

    this.authStateSubject.next({
      user: null,
      token: null,
      isAuthenticated: false,
    });
  }

  private loadStoredAuth(): void {
    const token = localStorage.getItem(this.TOKEN_KEY);
    const userJson = localStorage.getItem(this.USER_KEY);

    if (token) {
      let user: User | null = null;

      if (userJson) {
        try {
          user = JSON.parse(userJson);
        } catch (error) {
          localStorage.removeItem(this.USER_KEY);
        }
      }

      this.authStateSubject.next({
        user,
        token,
        isAuthenticated: true,
      });
    }
  }

  getToken(): string | null {
    return this.authStateSubject.value.token;
  }

  getUser(): User | null {
    return this.authStateSubject.value.user;
  }

  getUserRole(): string | null {
    return this.authStateSubject.value.user?.role || null;
  }

  isAuthenticated(): boolean {
    return this.authStateSubject.value.isAuthenticated;
  }

  refreshToken(): Observable<any> {
    return this.authApi.refreshToken();
  }

  getCurrentUser(): Observable<any> {
    return this.authApi.getCurrentUser();
  }

  redirectAfterLogin(): void {
    const role = this.getUserRole();
    switch (role) {
      case 'User':
        this.router.navigate(['/']);
        break;
      case 'Seller':
        this.router.navigate(['/seller/dashboard']);
        break;
      case 'Consultant':
        this.router.navigate(['/consultant/conversations']);
        break;
      case 'Admin':
        this.router.navigate(['/admin/dashboard']);
        break;
      default:
        this.router.navigate(['/']);
        break;
    }
  }
}
