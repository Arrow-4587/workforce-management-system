import { NgIf } from '@angular/common';
import { Component, inject, ChangeDetectorRef } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { AuthService } from '../../core/services/auth.service';

@Component({
  standalone: true,
  imports: [ReactiveFormsModule, NgIf],
  templateUrl: './login.page.html',
  styleUrls: ['./login.page.css']
})
export class LoginPageComponent {
  private readonly fb = inject(FormBuilder);
  private readonly auth = inject(AuthService);
  private readonly router = inject(Router);
  private readonly route = inject(ActivatedRoute);
  private readonly cdr = inject(ChangeDetectorRef);

  protected loading = false;
  protected errorMessage = '';

  protected readonly form = this.fb.nonNullable.group({
    username: ['', [Validators.required]],
    password: ['', [Validators.required]]
  });

  clearError(): void {
    if (this.errorMessage) {
      this.errorMessage = '';
      this.cdr.detectChanges();
    }
  }

  submit(): void {
    this.errorMessage = '';

    if (this.form.invalid) {
      this.form.markAllAsTouched();
      this.cdr.detectChanges();
      return;
    }

    this.loading = true;
    this.cdr.detectChanges();
    this.auth.login(this.form.getRawValue()).subscribe({
      next: response => {
        this.loading = false;
        if (response.isFirstLogin) {
          this.router.navigate(['/change-password']);
          return;
        }

        const returnUrl = this.route.snapshot.queryParamMap.get('returnUrl');
        this.router.navigateByUrl(returnUrl ?? '/dashboard');
      },
      error: error => {
        this.loading = false;
        
        let msg = 'Unable to sign in. Please verify your credentials and try again.';
        if (error?.error) {
          if (typeof error.error === 'string') {
            msg = error.error;
          } else if (error.error.message) {
            msg = error.error.message;
          } else if (error.message) {
            msg = error.message;
          }
        } else if (error?.message) {
          msg = error.message;
        }

        this.errorMessage = msg;
        this.cdr.detectChanges();
      }
    });
  }
}
