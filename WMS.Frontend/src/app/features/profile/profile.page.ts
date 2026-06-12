import { DatePipe, NgIf } from '@angular/common';
import { Component, OnInit, inject, signal } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { ProfileService } from '../../core/services/profile.service';
import { ToastService } from '../../shared/components/toast.service';
import { AuthService } from '../../core/services/auth.service';
import { ProfileResponse } from '../../core/models/wms.models';

@Component({
  standalone: true,
  imports: [ReactiveFormsModule, NgIf, DatePipe],
  templateUrl: './profile.page.html',
  styleUrls: ['./profile.page.css']
})
export class ProfilePageComponent implements OnInit {
  private readonly fb = inject(FormBuilder);
  private readonly profileService = inject(ProfileService);
  private readonly toast = inject(ToastService);
  readonly auth = inject(AuthService);

  protected readonly profile = signal<ProfileResponse | null>(null);
  protected readonly savingProfile = signal(false);
  protected readonly savingPassword = signal(false);

  protected readonly profileForm = this.fb.nonNullable.group({
    firstName: ['', [Validators.required, Validators.maxLength(50)]],
    lastName: ['', [Validators.required, Validators.maxLength(50)]],
    email: ['', [Validators.required, Validators.email]],
    phoneNumber: ['', [Validators.required, Validators.pattern(/^\d{10,15}$/)]],
  });

  protected readonly passwordForm = this.fb.nonNullable.group({
    currentPassword: ['', [Validators.required]],
    newPassword: ['', [Validators.required, Validators.minLength(8)]],
    confirmPassword: ['', [Validators.required]],
  });

  ngOnInit(): void {
    this.profileService.getProfile().subscribe(p => {
      this.profile.set(p);
      this.profileForm.patchValue({ firstName: p.firstName, lastName: p.lastName, email: p.email, phoneNumber: p.phoneNumber });
    });
  }

  isRootAdmin(): boolean {
    return this.auth.getUsername()?.toLowerCase() === 'admin';
  }

  initials(): string {
    const p = this.profile();
    if (p) return `${p.firstName[0]}${p.lastName[0]}`.toUpperCase();
    return (this.auth.getUsername() ?? 'WM').slice(0, 2).toUpperCase();
  }

  pfInvalid(field: string): boolean {
    const ctrl = this.profileForm.get(field);
    return !!ctrl && ctrl.touched && ctrl.invalid;
  }

  pwInvalid(field: string): boolean {
    const ctrl = this.passwordForm.get(field);
    return !!ctrl && ctrl.touched && ctrl.invalid;
  }

  passwordMismatch(): boolean {
    const ctrl = this.passwordForm.get('confirmPassword');
    if (!ctrl || !ctrl.touched) return false;
    const { newPassword, confirmPassword } = this.passwordForm.getRawValue();
    return newPassword !== confirmPassword;
  }

  saveProfile(): void {
    if (this.profileForm.invalid) { this.profileForm.markAllAsTouched(); return; }
    this.savingProfile.set(true);
    this.profileService.updateProfile(this.profileForm.getRawValue()).subscribe({
      next: () => { this.savingProfile.set(false); this.toast.success('Profile updated', 'Your details have been saved.'); },
      error: err => { this.savingProfile.set(false); this.toast.error('Update failed', err?.error ?? 'Please try again.'); }
    });
  }

  changePassword(): void {
    if (this.passwordForm.invalid) { this.passwordForm.markAllAsTouched(); return; }
    const { newPassword, confirmPassword, currentPassword } = this.passwordForm.getRawValue();
    if (newPassword !== confirmPassword) { this.toast.error('Passwords do not match'); return; }
    this.savingPassword.set(true);
    this.profileService.changePassword({ currentPassword, newPassword }).subscribe({
      next: () => {
        this.savingPassword.set(false);
        this.passwordForm.reset();
        this.toast.success('Password changed', 'Your password has been updated successfully.');
      },
      error: err => { this.savingPassword.set(false); this.toast.error('Change failed', err?.error ?? 'Please try again.'); }
    });
  }
}
