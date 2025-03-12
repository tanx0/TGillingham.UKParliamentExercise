import { Component, OnInit } from '@angular/core';
import { PersonService } from '../../services/person.service';
import { PersonViewModel } from '../../models/person-view-model';
import { DepartmentService } from '../../services/department.service';
import { DepartmentViewModel } from '../../models/department-view-model';
import { Observable } from 'rxjs';

@Component({
  selector: 'bot-person',
  templateUrl: './person.component.html',
  styleUrls: ['./person.component.css'],
})
export class PersonComponent implements OnInit {
  people: PersonViewModel[] = [];
  //selectedPerson: PersonViewModel | null = null;
  defaultPerson: PersonViewModel = {
    id: 0,
    firstName: '',
    lastName: '',
    dateOfBirth: '',
    departmentId: 0, // Default to an invalid selection
  };

  selectedPerson: PersonViewModel = this.defaultPerson;

  departments: DepartmentViewModel[] = [];

  loading: boolean = false; // Track loading state
  saveError: boolean = false;
  isEditing: boolean = false;// Flag to control form visibility
  errorMessage: string = '';
  globalErrorMessage: string = '';
  validationErrors = {}; // Clear previous errors
  constructor(private personService: PersonService, private departmentService: DepartmentService) { }

  ngOnInit(): void {
    this.loadPeople();
    this.loadDepartments();
  }

  loadDepartments(): void {
    
    this.departmentService.getDepartments().subscribe((departments) => {
      this.departments = departments;      
    }, (error) => {
      console.error('Error loading departments:', error);
      this.loading = false;      
      this.handleGlobalApiError(error);
    });
  }

  departmentIdInvalid(value: number): boolean {
    return value === 0; // Check if departmentId is 0 (invalid)
  }

  loadPeople(): void {
    this.loading = true;
    this.saveError = false;
    this.personService.getPeople().subscribe((people) => {
      this.people = people;
      this.loading = false;
      this.saveError = false;
    }, (error) => {
      console.error('Error loading people:', error);
      this.loading = false;      
      this.handleGlobalApiError(error);
    });
  }

  selectPerson(person: PersonViewModel): void {
    this.selectedPerson = { ...person };
    this.isEditing = true; // Show the form when a person is selected
    this.validationErrors = {}; // Clear previous errors
  }

  cancelPersonEdit(): void {
    this.selectedPerson = this.defaultPerson; // Clear form data
    this.isEditing = false; // Hide form
    this.validationErrors = {}; // Clear previous errors
  }

  addNewPerson(): void {
    const newPerson: PersonViewModel = {
      id: 0,
      firstName: '',
      lastName: '',
      dateOfBirth: '',
      departmentId: 0  // Default to an empty departmentId (0)
    };
    this.selectedPerson = newPerson;
    this.isEditing = true; // Show the form when adding a new person
    this.errorMessage = '';
    this.validationErrors = {}; // Clear previous errors
  }

  savePerson(): void {
    if (this.selectedPerson) {
      this.loading = true;
      this.saveError = false;

      const createOrUpdate = this.selectedPerson.id === 0
        ? this.personService.createPerson(this.selectedPerson)
        : this.personService.updatePerson(this.selectedPerson);

      createOrUpdate.subscribe({
        next: () => {
          this.loadPeople(); // Refresh the list after successful save
          this.selectedPerson = this.defaultPerson; // Clear the form
          this.isEditing = false; // Hide the form
          this.loading = false;
        },
        error: (error) => {
          this.loading = false;
          this.isEditing = true;//keep form open
          this.handleApiError(error);
        }
      });
    }
  }

  handleApiError(error: any): void {
    console.error('API Error:', error);
    if (typeof error.error === 'string') {
      this.errorMessage = error.error; // the message from the backend response      
  } else if (error.status === 400 && error.error.errors) {
    // Validation errors (BadRequest) - store them in validationErrors
    this.validationErrors = this.flattenValidationErrors(error.error.errors);
    this.errorMessage = ''; // Clear generic error message since it's field-specific
    } else if (error.status === 404) {
      this.errorMessage = 'Resource not found';
    } else if (error.status === 500) {
      this.errorMessage = 'Server error. Please try again later.';
    } else {
      this.errorMessage = 'An unexpected error occurred. Please try again.';
    }
    this.saveError = true; // Show the error message 
  }

  private flattenValidationErrors(errors: any): { [key: string]: string } {
    let validationMessages: { [key: string]: string } = {};

    for (const field in errors) {
      if (errors[field]) {
        // If multiple errors exist for a field, join them into one string
        validationMessages[field] = Array.isArray(errors[field])
          ? errors[field].join(', ')
          : errors[field];
      }
    }

    return validationMessages;
  }

  handleGlobalApiError(error: any): void {
    console.error('API Error:', error);
    if (typeof error.error === 'string') {
      this.globalErrorMessage = error.error; 
    } else if (error.status === 500) {
      this.globalErrorMessage = 'Server error. Please try again later.';
    } else {
      this.globalErrorMessage = 'An unexpected error occurred. Please try again.';
    }

  }

  // Get the department name from the departmentId
  getDepartmentName(departmentId: number): string {
    const department = this.departments.find(dept => dept.departmentId === departmentId);
    return department ? department.departmentName : 'Unknown';
  }
}


