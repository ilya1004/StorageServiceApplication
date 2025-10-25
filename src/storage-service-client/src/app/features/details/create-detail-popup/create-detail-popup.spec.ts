import { ComponentFixture, TestBed } from '@angular/core/testing';

import { CreateDetailPopup } from './create-detail-popup';

describe('CreateDetailPopup', () => {
  let component: CreateDetailPopup;
  let fixture: ComponentFixture<CreateDetailPopup>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [CreateDetailPopup]
    })
    .compileComponents();

    fixture = TestBed.createComponent(CreateDetailPopup);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
