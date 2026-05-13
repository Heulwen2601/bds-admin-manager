export interface Lead {
  id: string;
  propertyId: string;
  userId?: string;
  fullName?: string;
  phone?: string;
  email?: string;
  guestName?: string;
  guestPhone?: string;
  guestEmail?: string;
  message?: string;
  status?: string;
  createdAt: string;
}

export interface CreateLeadRequest {
  fullName: string;
  phone: string;
  email?: string;
  message?: string;
  guestName?: string;
  guestPhone?: string;
  guestEmail?: string;
}
