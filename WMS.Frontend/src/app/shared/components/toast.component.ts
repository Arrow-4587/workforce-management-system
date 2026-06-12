import { NgClass, NgFor, NgIf } from '@angular/common';
import { Component, inject } from '@angular/core';
import { ToastService, Toast } from './toast.service';

@Component({
  selector: 'app-toast',
  standalone: true,
  imports: [NgFor, NgIf, NgClass],
  templateUrl: './toast.component.html'
})
export class ToastComponent {
  readonly toastService = inject(ToastService);
  trackById(_: number, t: Toast): number { return t.id; }
}
