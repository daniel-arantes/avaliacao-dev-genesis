export interface CalculateCdbRequest { initialValue: number; months: number; }
export interface CdbCalculation {
  initialValue: number; months: number; grossAmount: number; grossEarnings: number;
  taxRate: number; taxAmount: number; netAmount: number;
}
