import { TestBed } from '@angular/core/testing';

import { PropertyApi } from './property-api';

describe('PropertyApi', () => {
  let service: PropertyApi;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(PropertyApi);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
