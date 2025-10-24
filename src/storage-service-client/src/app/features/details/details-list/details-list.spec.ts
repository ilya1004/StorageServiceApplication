import { ComponentFixture, TestBed } from '@angular/core/testing';

import { DetailsList } from './details-list';

describe('DetailsList', () => {
  let component: DetailsList;
  let fixture: ComponentFixture<DetailsList>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [DetailsList]
    })
    .compileComponents();

    fixture = TestBed.createComponent(DetailsList);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
