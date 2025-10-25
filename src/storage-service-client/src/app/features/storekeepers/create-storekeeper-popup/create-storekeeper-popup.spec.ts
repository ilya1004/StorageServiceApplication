import { ComponentFixture, TestBed } from '@angular/core/testing';

import { CreateStorekeeperPopup } from './create-storekeeper-popup';

describe('CreateStorekeeperPopup', () => {
  let component: CreateStorekeeperPopup;
  let fixture: ComponentFixture<CreateStorekeeperPopup>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [CreateStorekeeperPopup]
    })
    .compileComponents();

    fixture = TestBed.createComponent(CreateStorekeeperPopup);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
