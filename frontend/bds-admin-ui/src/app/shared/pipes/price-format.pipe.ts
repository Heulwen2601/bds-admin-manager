import { Pipe, PipeTransform } from '@angular/core';

@Pipe({
  name: 'priceFormat',
  standalone: true,
})
export class PriceFormatPipe implements PipeTransform {
  transform(value?: number | null): string {
    if (value == null || value <= 0) {
      return 'Thỏa thuận';
    }

    if (value >= 1_000_000_000) {
      return `${this.formatDecimal(value / 1_000_000_000)} tỷ`;
    }

    if (value >= 1_000_000) {
      return `${this.formatDecimal(value / 1_000_000)} triệu`;
    }

    return `${this.formatNumber(value)} đ`;
  }

  private formatDecimal(value: number): string {
    const formatted = new Intl.NumberFormat('vi-VN', {
      minimumFractionDigits: 0,
      maximumFractionDigits: 1,
    }).format(value);
    return formatted.replace(',0', '');
  }

  private formatNumber(value: number): string {
    return new Intl.NumberFormat('vi-VN', {
      maximumFractionDigits: 0,
    }).format(value);
  }
}
