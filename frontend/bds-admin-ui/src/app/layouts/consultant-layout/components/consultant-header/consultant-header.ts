import { Component, inject } from '@angular/core';
import { Router } from '@angular/router';
import { CommonModule } from '@angular/common';
import { AuthService } from '../../../../core/services/auth.service';
import { NotificationDropdownComponent } from '../../../../shared/components/notification-dropdown/notification-dropdown';

@Component({
  selector: 'app-consultant-header',
  standalone: true,
  imports: [CommonModule, NotificationDropdownComponent],
  templateUrl: './consultant-header.html',
  styleUrl: './consultant-header.scss',
})
export class ConsultantHeaderComponent {
  authService = inject(AuthService);
  router = inject(Router);

  logout() {
    this.authService.logout();
  }
}
