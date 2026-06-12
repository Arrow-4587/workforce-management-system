import { Injectable, signal } from '@angular/core';

export interface ConfirmState {
  isOpen: boolean;
  title: string;
  message: string;
  confirmText: string;
  cancelText: string;
  onConfirm: () => void;
  onCancel?: () => void;
  isDestructive: boolean;
}

@Injectable({
  providedIn: 'root'
})
export class ConfirmService {
  readonly state = signal<ConfirmState>({
    isOpen: false,
    title: '',
    message: '',
    confirmText: 'Confirm',
    cancelText: 'Cancel',
    onConfirm: () => {},
    isDestructive: false
  });

  open(options: Omit<ConfirmState, 'isOpen'>): void {
    this.state.set({
      ...options,
      isOpen: true
    });
  }

  confirm(): void {
    const currentState = this.state();
    if (currentState.isOpen) {
      currentState.onConfirm();
      this.close();
    }
  }

  cancel(): void {
    const currentState = this.state();
    if (currentState.isOpen) {
      if (currentState.onCancel) {
        currentState.onCancel();
      }
      this.close();
    }
  }

  close(): void {
    this.state.update(s => ({ ...s, isOpen: false }));
  }
}
