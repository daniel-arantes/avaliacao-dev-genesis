import { ComponentFixture, TestBed } from '@angular/core/testing';
import { of, throwError } from 'rxjs';
import { App } from './app';
import { InvestmentApiService } from './investment-api.service';

describe('App', () => {
  const calculation = {
    initialValue: 1000,
    months: 2,
    grossAmount: 1019.53,
    grossEarnings: 19.53,
    taxRate: 0.225,
    taxAmount: 4.39,
    netAmount: 1015.14,
  };

  let api: { calculate: ReturnType<typeof vi.fn> };

  beforeEach(async () => {
    api = { calculate: vi.fn().mockReturnValue(of(calculation)) };

    await TestBed.configureTestingModule({
      imports: [App],
      providers: [{ provide: InvestmentApiService, useValue: api }],
    }).compileComponents();
  });

  it('creates the calculator and keeps submission disabled while invalid', () => {
    const fixture = createComponent();

    expect(element(fixture, '#calculator-title').textContent).toContain('Dados da aplica');
    expect(element<HTMLButtonElement>(fixture, 'button').disabled).toBe(true);
  });

  it('submits valid values and displays gross and net results', () => {
    const fixture = createComponent();
    fillSimulation(fixture, '1000', '2');

    element<HTMLFormElement>(fixture, 'form').dispatchEvent(new Event('submit'));
    fixture.detectChanges();

    expect(api.calculate).toHaveBeenCalledWith({ initialValue: 1000, months: 2 });
    expect(element(fixture, '.results').textContent).toContain('1.019,53');
    expect(element(fixture, '.results').textContent).toContain('1.015,14');
  });

  it('shows an accessible message when the API fails', () => {
    api.calculate.mockReturnValue(throwError(() => new Error('network')));
    const fixture = createComponent();
    fillSimulation(fixture, '1000', '12');

    element<HTMLFormElement>(fixture, 'form').dispatchEvent(new Event('submit'));
    fixture.detectChanges();

    expect(element(fixture, '[role="alert"]').textContent).toContain('Não foi possível');
  });
});

function createComponent(): ComponentFixture<App> {
  const fixture = TestBed.createComponent(App);
  fixture.detectChanges();
  return fixture;
}

function fillSimulation(fixture: ComponentFixture<App>, initialValue: string, months: string): void {
  setInput(fixture, '#initialValue', initialValue);
  setInput(fixture, '#months', months);
}

function setInput(fixture: ComponentFixture<App>, selector: string, value: string): void {
  const input = element<HTMLInputElement>(fixture, selector);
  input.value = value;
  input.dispatchEvent(new Event('input'));
}

function element<T extends HTMLElement = HTMLElement>(fixture: ComponentFixture<App>, selector: string): T {
  const result = fixture.nativeElement.querySelector(selector) as T | null;
  if (!result) throw new Error(`Element not found: ${selector}`);
  return result;
}
