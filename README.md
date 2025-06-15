# UniEvents - University Event Management Platform

**UniEvents** is a comprehensive, full-stack web application designed as a graduation project. It serves as a centralized hub for managing and discovering university events, bridging the gap between event organizers and attendees on campus.

---

## Live Demo

You can access a live demo of the application here: **[http://unievents.runasp.net/]**

---

## Key Features

The platform is built with two main user roles in mind: Attendees and Organizers.

### For Attendees (Students & Faculty):
*   **Event Exploration:** A dynamic homepage to discover upcoming events with advanced search and filtering capabilities (by date, location, category).
*   **Detailed Event View:** A comprehensive details page for each event, including description, media, location, and schedule.
*   **Booking System:** Simple one-click registration and cancellation for events.
*   **Personalization:**
    *   **My Favorites:** Users can save events they are interested in.
    *   **My Bookings:** A dedicated page to view all registered events.
    *   **Personal Calendar:** An interactive weekly calendar view displaying the user's registered events.
*   **Interactive Feedback:** Users can post comments and ratings on events they have attended.
*   **Real-time Support:** Integrated live chat support using **Tawk.to** for instant help.

### For Organizers:
*   **Multi-Step Event Creation:** An intuitive, guided wizard to create new events, including basic info, media uploads (images/videos), and invitation details.
*   **Event Management:** A "My Events" dashboard to view, edit, and manage all created events.
*   **Event Analytics:** [اختياري: إذا أكملت هذه الميزة] A dashboard to view key metrics for their events, such as attendee count, ratings, and feedback.
*   **Approval Workflow:** Events submitted by organizers are set to a "Pending" status, awaiting admin approval before being published.

---

## Technology Stack

This project was built using a modern, robust technology stack:

*   **Backend:**
    *   **Framework:** ASP.NET Core MVC 6.0
    *   **Language:** C#
    *   **ORM:** Entity Framework Core (Database-First Approach)
    *   **Database:** Microsoft SQL Server
*   **Frontend:**
    *   **Templating:** Razor Pages
    *   **Styling:** HTML5, CSS3, Bootstrap 5
    *   **JavaScript:** Vanilla JS & jQuery for DOM manipulation and AJAX requests.
*   **Third-Party Libraries & Services:**
    *   **SweetAlert2:** For beautiful, interactive pop-up messages.
    *   **Tawk.to:** For the live chat support widget.
    *   **WOW.js:** For scroll-triggered animations.

---

## Setup and Installation
To run this project locally, follow these steps:

1.  **Clone the repository:**
    ```bash
    git clone https://github.com/abutouqyanal/UniEvents.git
    ```
2.  **Database Setup:**
    *   Open the project in Visual Studio.
    *   Navigate to `appsettings.json`.
    *   Modify the `ConnectionString` to point to your local SQL Server instance.
    *   Open the Package Manager Console and run `Update-Database` to apply migrations and create the database schema.
3.  **Run the application:**
    *   Press `F5` or click the "Run" button in Visual Studio.

---

## Author

***[Yanal Abutouq]** - [www.linkedin.com/in/yanal-abutouq-78b08a305]
---
##  Acknowledgments

I would like to express my sincere gratitude to my project supervisor, **[Dr.Raed Shatnawi]**, for their invaluable guidance and support throughout this project.
