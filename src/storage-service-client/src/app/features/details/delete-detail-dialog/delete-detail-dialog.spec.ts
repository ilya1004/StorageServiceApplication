import { ComponentFixture, TestBed } from '@angular/core/testing';

import { DeleteDetailDialog } from './delete-detail-dialog';

describe('DeleteDetailDialog', () => {
  let component: DeleteDetailDialog;
  let fixture: ComponentFixture<DeleteDetailDialog>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [DeleteDetailDialog]
    })
    .compileComponents();

    fixture = TestBed.createComponent(DeleteDetailDialog);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
