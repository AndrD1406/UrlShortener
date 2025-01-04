import { Component, OnInit } from '@angular/core';
import {FormArray, FormControl, FormGroup, ReactiveFormsModule, Validators} from '@angular/forms';
import { Router } from '@angular/router';
import { UrlService } from '../../services/url/url.service';
import { Url } from '../../models/url/url';
import {DatePipe, NgForOf, NgIf} from '@angular/common';
import { CommonModule } from '@angular/common';
import {UrlCreate} from '../../models/UrlCreate/url-create';

@Component({
  selector: 'app-url',
  standalone: true,
  templateUrl: './url.component.html',
  imports: [
    ReactiveFormsModule,
    DatePipe,
    NgIf,
    NgForOf
  ],
  styleUrls: ['./url.component.css']
})
export class UrlComponent implements OnInit {
  urls: Url[] = [];
  urlForm: FormGroup;
  putUrlForm: FormGroup;
  isFormSubmitted: boolean = false;
  isUrlValid: boolean = true;

  constructor(private urlService: UrlService, private router: Router) {
    this.urlForm = new FormGroup({
      longUrl: new FormControl(null, [Validators.required, Validators.pattern('https?://.+')])
    });

    // Initialize putUrlForm
    this.putUrlForm = new FormGroup({
      urls: new FormArray([])
    });
  }

  ngOnInit(): void {
    this.loadUrls();
  }

  loadUrls(): void {
    this.urlService.getAll().subscribe({
      next: (response: Url[]) => {
        this.urls = response;

        const urlsArray = this.putUrlForm.get('urls') as FormArray;
        urlsArray.clear();

        this.urls.forEach(url => {
          urlsArray.push(new FormGroup({
            shortUrl: new FormControl(url.shortUrl, [Validators.required]),
            longUrl: new FormControl(url.longUrl, [Validators.required]),
            createdDate: new FormControl(url.createdDate, [Validators.required]),
            createdBy: new FormControl(url.createdBy || 'N/A', [Validators.required])
          }));
        });

        console.log(this.putUrlForm);
      },
      error: (error: any) => {
        console.error('Error loading URLs:', error);
      }
    });
  }


  refreshClicked(): void {
    this.loadUrls();
  }

  onAddUrl(): void {
    this.isFormSubmitted = true;
    if (this.urlForm.valid) {
      const longUrl = new UrlCreate(this.urlForm.value.longUrl);
      this.urlService.create(longUrl).subscribe({
        next: (response: Url) => {
          console.log('URL added successfully:', response);
          this.urlForm.reset();
          this.isFormSubmitted = false;
          this.isUrlValid = true;
          this.loadUrls(); // Refresh the list of URLs
        },
        error: (error) => {
          console.error('Error adding URL:', error);
          this.isUrlValid = false;
        }
      });
    }
  }

  get longUrlControl() {
    return this.urlForm.controls['longUrl'];
  }

  get putUrlFormUlrs(): FormArray {
    return this.putUrlForm.get('urls') as FormArray || new FormArray([]);
  }
}
