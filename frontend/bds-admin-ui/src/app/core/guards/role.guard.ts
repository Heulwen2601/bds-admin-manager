import { Injectable } from '@angular/core';
import { CanActivate, Router, ActivatedRouteSnapshot } from '@angular/router';
import { AuthService } from '../services/auth.service';

@Injectable({
  providedIn: 'root'
})
export class RoleGuard implements CanActivate {
  constructor(
    private authService: AuthService,
    private router: Router
  ) {}

  canActivate(route: ActivatedRouteSnapshot): boolean {
    const requiredRoles = route.data['roles'] as string[];

    if (!requiredRoles || requiredRoles.length === 0) {
      return true;
    }

    const userRole = this.authService.getUserRole();

    if (!userRole || !requiredRoles.includes(userRole)) {
      // Redirect based on user role
      this.redirectByRole(userRole);
      return false;
    }

    return true;
  }

  private redirectByRole(role: string | null): void {
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
        this.router.navigate(['/auth/login']);
        break;
    }
  }
}