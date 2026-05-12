import { Component, inject } from '@angular/core';
import { Router } from '@angular/router';
import { CommonModule } from '@angular/common';
import { AuthService } from '../../../../core/services/auth.service';
import { NotificationDropdownComponent } from '../../../../shared/components/notification-dropdown/notification-dropdown';

@Component({
  selector: 'app-admin-header',
  standalone: true,
  imports: [CommonModule, NotificationDropdownComponent],
  templateUrl: './admin-header.html',
  styleUrl: './admin-header.scss',
})
export class AdminHeaderComponent {
  authService = inject(AuthService);
  router = inject(Router);

  logout() {
    this.authService.logout();
  }
}
