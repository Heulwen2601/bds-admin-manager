import { Component, inject } from '@angular/core';
import { Router } from '@angular/router';
import { CommonModule } from '@angular/common';
import { AuthService } from '../../../../core/services/auth.service';
import { NotificationDropdownComponent } from '../../../../shared/components/notification-dropdown/notification-dropdown';

@Component({
  selector: 'app-seller-header',
  standalone: true,
  imports: [CommonModule, NotificationDropdownComponent],
  templateUrl: './seller-header.html',
  styleUrl: './seller-header.scss'
})
export class SellerHeaderComponent {
  authService = inject(AuthService);
  router = inject(Router);

  logout() {
    this.authService.logout();
  }
}


