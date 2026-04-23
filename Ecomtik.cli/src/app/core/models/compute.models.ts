export interface IntegrationMethod {
  id: number;
  code: string;
  name: string;
}

export interface ComputeRequestBody {
  function: string;
  method: string;
  a: number;
  b: number;
  steps: number;
}

export interface ComputeResponseBody {
  result: number;
  durationMs: number;
  id: number;
}

export interface HistoryEntry {
  id: number;
  functionExpr: string;
  lowerBound: number;
  upperBound: number;
  method: string;
  steps: number;
  result: number;
  durationMs: number;
  createdAt: string;
}
