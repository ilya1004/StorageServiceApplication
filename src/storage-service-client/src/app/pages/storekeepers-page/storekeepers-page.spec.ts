import { ComponentFixture, TestBed } from '@angular/core/testing';

import { StorekeepersPage } from './storekeepers-page';

describe('StorekeepersPage', () => {
  let component: StorekeepersPage;
  let fixture: ComponentFixture<StorekeepersPage>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [StorekeepersPage]
    })
    .compileComponents();

    fixture = TestBed.createComponent(StorekeepersPage);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
