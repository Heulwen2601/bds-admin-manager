export interface Lead {
  id: string;
  propertyId: string;
  userId?: string;
  guestName?: string;
  guestPhone?: string;
  guestEmail?: string;
  message?: string;
  status: string;
  createdAt: string;
}

export interface CreateLeadRequest {
  guestName?: string;
  guestPhone?: string;
  guestEmail?: string;
  message?: string;
}
