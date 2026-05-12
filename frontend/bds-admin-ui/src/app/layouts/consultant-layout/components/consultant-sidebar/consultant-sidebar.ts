import { Component } from '@angular/core';
import { RouterLink, RouterLinkActive } from '@angular/router';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-consultant-sidebar',
  standalone: true,
  imports: [CommonModule, RouterLink, RouterLinkActive],
  templateUrl: './consultant-sidebar.html',
  styleUrl: './consultant-sidebar.scss'
})
export class ConsultantSidebarComponent {}

