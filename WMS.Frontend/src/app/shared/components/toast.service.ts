import { Injectable, signal } from '@angular/core';

export type ToastType = 'success' | 'error' | 'warning' | 'info';

export interface Toast {
  id: number;
  type: ToastType;
  title: string;
  message?: string;
  leaving?: boolean;
}

@Injectable({ providedIn: 'root' })
export class ToastService {
  private nextId = 0;
  readonly toasts = signal<Toast[]>([]);

  success(title: string, message?: string): void {
    this.add({ type: 'success', title, message });
  }

  error(title: string, message?: string): void {
    this.add({ type: 'error', title, message });
  }

  warning(title: string, message?: string): void {
    this.add({ type: 'warning', title, message });
  }

  info(title: string, message?: string): void {
    this.add({ type: 'info', title, message });
  }

  dismiss(id: number): void {
    this.toasts.update(list =>
      list.map(t => t.id === id ? { ...t, leaving: true } : t)
    );
    setTimeout(() => {
      this.toasts.update(list => list.filter(t => t.id !== id));
    }, 280);
  }

  private add(toast: Omit<Toast, 'id'>): void {
    const id = ++this.nextId;
    this.toasts.update(list => [...list, { ...toast, id }]);
    setTimeout(() => this.dismiss(id), 4500);
  }
}
