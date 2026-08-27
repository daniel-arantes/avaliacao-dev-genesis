import { CurrencyPipe, PercentPipe, registerLocaleData } from '@angular/common';
import localePt from '@angular/common/locales/pt';
import { ChangeDetectionStrategy, Component, inject, signal } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { finalize } from 'rxjs';
import { InvestmentApiService } from './investment-api.service';
import { CdbCalculation } from './investment.models';

registerLocaleData(localePt);

@Component({
  selector: 'app-root', imports: [ReactiveFormsModule, CurrencyPipe, PercentPipe],
  templateUrl: './app.html', styleUrl: './app.scss', changeDetection: ChangeDetectionStrategy.OnPush
})
export class App {
  private readonly formBuilder = inject(FormBuilder);
  private readonly investmentApi = inject(InvestmentApiService);
  protected readonly result = signal<CdbCalculation | null>(null);
  protected readonly loading = signal(false);
  protected readonly errorMessage = signal<string | null>(null);
  protected readonly form = this.formBuilder.group({
    initialValue: [null as number | null, [Validators.required, Validators.min(0.01)]],
    months: [null as number | null, [Validators.required, Validators.min(2), Validators.pattern(/^\d+$/)]]
  });

  protected calculate(): void {
    if (this.form.invalid) { this.form.markAllAsTouched(); return; }
    const { initialValue, months } = this.form.getRawValue();
    if (initialValue === null || months === null) return;
    this.loading.set(true); this.errorMessage.set(null); this.result.set(null);
    this.investmentApi.calculate({ initialValue, months }).pipe(finalize(() => this.loading.set(false))).subscribe({
      next: result => this.result.set(result),
      error: () => this.errorMessage.set('Não foi possível calcular agora. Confira se a API está em execução e tente novamente.')
    });
  }
}
