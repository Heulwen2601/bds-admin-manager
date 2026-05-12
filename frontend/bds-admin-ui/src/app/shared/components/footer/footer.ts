import { CommonModule } from '@angular/common';
import { Component } from '@angular/core';
import { RouterLink } from '@angular/router';

interface FooterLink {
  label: string;
  route: string;
}

interface BranchOffice {
  name: string;
  address: string;
  hotline: string;
}

@Component({
  selector: 'app-footer',
  standalone: true,
  imports: [CommonModule, RouterLink],
  templateUrl: './footer.html',
  styleUrl: './footer.scss'
})
export class FooterComponent {
  guideLinks: FooterLink[] = [
    { label: 'Giới thiệu', route: '/directory' },
    { label: 'Bảng giá & hỗ trợ', route: '/directory' },
    { label: 'Câu hỏi thường gặp', route: '/directory' },
    { label: 'Báo lỗi / góp ý', route: '/directory' },
    { label: 'Sơ đồ trang', route: '/directory' }
  ];

  policyLinks: FooterLink[] = [
    { label: 'Quy định đăng tin', route: '/directory' },
    { label: 'Quy chế hoạt động', route: '/directory' },
    { label: 'Điều khoản sử dụng', route: '/directory' },
    { label: 'Chính sách bảo mật', route: '/directory' },
    { label: 'Giải quyết khiếu nại', route: '/directory' }
  ];

  branches: BranchOffice[] = [
    {
      name: 'Chi nhánh TP. Hồ Chí Minh',
      address: 'Tầng 2, tòa Viettel, 285 Cách Mạng Tháng Tám, TP. Hồ Chí Minh',
      hotline: 'Tổng đài: 1900 1881'
    },
    {
      name: 'Chi nhánh Đà Nẵng',
      address: 'Tầng 9, Vinh Trung Plaza, 255-257 Hùng Vương, TP. Đà Nẵng',
      hotline: 'Tổng đài: 1900 1881'
    },
    {
      name: 'Chi nhánh Hải Phòng',
      address: 'Phòng 502, TD Business Center, đường Lê Hồng Phong, TP. Hải Phòng',
      hotline: 'Tổng đài: 1900 1881'
    },
    {
      name: 'Chi nhánh Vũng Tàu',
      address: 'Tòa ACB, 111 Hoàng Hoa Thám, TP. Vũng Tàu',
      hotline: 'Tổng đài: 1900 1881'
    },
    {
      name: 'Chi nhánh Bình Dương',
      address: 'Biconsi Tower, đường Phú Lợi, TP. Thủ Dầu Một, Bình Dương',
      hotline: 'Tổng đài: 1900 1881'
    },
    {
      name: 'Chi nhánh Nha Trang',
      address: '11 Lý Thánh Tôn, phường Nha Trang, tỉnh Khánh Hòa',
      hotline: 'Tổng đài: 1900 1881'
    }
  ];
}

