import { NgIf } from '@angular/common';
import { Component, inject } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { AuthService } from '../../core/services/auth.service';

@Component({
  standalone: true,
  imports: [ReactiveFormsModule, NgIf],
  templateUrl: './change-password.page.html',
  styleUrls: ['./change-password.page.css']
})
export class ChangePasswordPageComponent {
  private readonly fb = inject(FormBuilder);
  private readonly auth = inject(AuthService);

  protected loading = false;
  protected errorMessage = '';

  protected readonly form = this.fb.nonNullable.group({
    currentPassword: ['', [Validators.required]],
    newPassword: ['', [
      Validators.required, 
      Validators.minLength(8),
      Validators.pattern(/(?=.*[a-z])(?=.*[A-Z])(?=.*[0-9])(?=.*[^a-zA-Z0-9])/)
    ]],
    confirmPassword: ['', [Validators.required]]
  });

  get newPasswordErrors() {
    const ctrl = this.form.get('newPassword');
    if (ctrl?.invalid && (ctrl.dirty || ctrl.touched)) {
      if (ctrl.hasError('required')) return 'New password is required.';
      if (ctrl.hasError('minlength')) return 'Password must be at least 8 characters.';
      if (ctrl.hasError('pattern')) return 'Password must contain at least one uppercase, one lowercase, one number, and one special character.';
    }
    return null;
  }

  submit(): void {
    this.errorMessage = '';

    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }

    const { currentPassword, newPassword, confirmPassword } = this.form.getRawValue();

    if (newPassword !== confirmPassword) {
      this.errorMessage = 'New password and confirmation must match.';
      return;
    }

    this.loading = true;
    this.auth.changePassword({ currentPassword, newPassword }).subscribe({
      next: () => {
        this.loading = false;
        this.auth.logout();
      },
      error: error => {
        this.loading = false;
        let msg = 'Unable to update password.';
        if (error?.error) {
          if (typeof error.error === 'string') {
            // Extract the actual error message from the ASP.NET Core System.Exception stack trace
            const exceptionMatch = error.error.match(/^System\.Exception:\s*(.+?)(?:\r?\n|$)/);
            if (exceptionMatch) {
              msg = exceptionMatch[1];
            } else {
              msg = error.error;
            }
          } else if (error.error.message) {
            msg = error.error.message;
          } else if (error.message) {
            msg = error.message;
          }
        } else if (error?.message) {
          msg = error.message;
        }
        this.errorMessage = msg;
      }
    });
  }
}
