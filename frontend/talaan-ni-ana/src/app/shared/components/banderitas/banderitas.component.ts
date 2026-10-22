import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';

const FLAG_COUNT = 120;

@Component({
  selector: 'app-banderitas',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './banderitas.component.html',
  styles: ':host { display: block; }'
})
export class BanderitasComponent {
  readonly flags = Array.from({ length: FLAG_COUNT });
}
