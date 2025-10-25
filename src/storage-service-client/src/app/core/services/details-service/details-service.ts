import {inject, Injectable} from '@angular/core';
import {ApiService} from '../api-service';
import {Observable} from 'rxjs';
import {Detail} from '../../models/details/detail';
import {CreateDetailDto} from '../../models/details/createDetailDto';

@Injectable({
  providedIn: 'root'
})
export class DetailsService {
  private readonly endpoint: string = 'details';

  apiService: ApiService = inject(ApiService);

  getAll() : Observable<Detail[]> {
    return this.apiService.get<Detail[]>(this.endpoint);
  }

  create(createDetailDto: CreateDetailDto) : Observable<Detail> {
    return this.apiService.post<Detail>(this.endpoint, createDetailDto);
  }

  delete(id: number) : Observable<void> {
    return this.apiService.delete<void>(this.endpoint);
  }
}
