import { Pipe, PipeTransform } from '@angular/core';

@Pipe({ name: 'peso', standalone: true })
export class PesoPipe implements PipeTransform {
  transform(value: number): string {
    return `₱${value.toLocaleString('en-PH', {
      minimumFractionDigits: 2,
      maximumFractionDigits: 2
    })}`;
  }
}