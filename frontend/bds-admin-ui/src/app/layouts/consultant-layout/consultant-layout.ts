import { Component } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { ConsultantSidebarComponent } from './components/consultant-sidebar/consultant-sidebar';
import { ConsultantHeaderComponent } from './components/consultant-header/consultant-header';

@Component({
  selector: 'app-consultant-layout',
  standalone: true,
  imports: [RouterOutlet, ConsultantSidebarComponent, ConsultantHeaderComponent],
  templateUrl: './consultant-layout.html',
  styleUrl: './consultant-layout.scss',
})
export class ConsultantLayoutComponent {}
