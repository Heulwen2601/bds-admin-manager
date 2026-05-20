import { CommonModule } from '@angular/common';
import { Component, ElementRef, HostListener, inject } from '@angular/core';
import { Router } from '@angular/router';
import { AuthService } from '../../../../core/services/auth.service';
import { NotificationDropdownComponent } from '../../../../shared/components/notification-dropdown/notification-dropdown';

@Component({
  selector: 'app-seller-header',
  standalone: true,
  imports: [CommonModule, NotificationDropdownComponent],
  templateUrl: './seller-header.html',
  styleUrl: './seller-header.scss',
})
export class SellerHeaderComponent {
  authService = inject(AuthService);
  private router = inject(Router);
  private elementRef = inject(ElementRef<HTMLElement>);

  accountMenuOpen = false;

  @HostListener('document:click', ['$event'])
  onDocumentClick(event: MouseEvent) {
    if (!this.accountMenuOpen) return;

    const target = event.target as Node | null;
    const targetElement = target instanceof Element ? target : target?.parentElement;
    const isInsideAccountMenu =
      !!targetElement?.closest('.seller-account-menu') &&
      this.elementRef.nativeElement.contains(targetElement);

    if (!isInsideAccountMenu) {
      this.closeAccountMenu();
    }
  }

  @HostListener('document:keydown.escape')
  onEscape() {
    this.closeAccountMenu();
  }

  get userDisplayName(): string {
    const user = this.authService.getUser();
    return user?.fullName || user?.email || 'Tài khoản';
  }

  get userInitial(): string {
    return this.userDisplayName.trim().charAt(0).toUpperCase() || 'U';
  }

  toggleAccountMenu(event: MouseEvent) {
    event.stopPropagation();
    this.accountMenuOpen = !this.accountMenuOpen;
  }

  closeAccountMenu() {
    this.accountMenuOpen = false;
  }

  navigateTo(route: string) {
    this.closeAccountMenu();
    this.router.navigateByUrl(route);
  }

  navigateToPasswordSettings() {
    this.closeAccountMenu();
    this.router.navigate(['/seller/profile'], { fragment: 'password' });
  }

  logout() {
    this.closeAccountMenu();
    this.authService.logout();
  }
}
