import { Component } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { SellerSidebarComponent } from './components/seller-sidebar/seller-sidebar';
import { SellerHeaderComponent } from './components/seller-header/seller-header';

@Component({
  selector: 'app-seller-layout',
  standalone: true,
  imports: [RouterOutlet, SellerSidebarComponent, SellerHeaderComponent],
  templateUrl: './seller-layout.html',
  styleUrl: './seller-layout.scss'
})
export class SellerLayoutComponent {}


