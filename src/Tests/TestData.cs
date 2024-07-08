using System.Security.Claims;
using UserAuthNOrg.Core.Models;

namespace Tests
{
    public static class TestData
    {
        public static readonly string UserId = "2a8fdbe3-14a4-0cda-8d36-39fa18266952";

        public static ClaimsPrincipal GetAuthenticatedUser()
        {
            return new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
                new Claim("http://schemas.microsoft.com/ws/2008/06/identity/claims/role", "admin"),
                new Claim("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier", TestData.UserId),
                new Claim("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress", "admin@test.com"),
                new Claim("jti", Guid.NewGuid().ToString())
            }, "TestAuthentication"));
        }

        public static List<User> GetUsers()
        {
            return new List<User>
            {
                new User
                {
                    FirstName = "Ekene",
                    Email = "dev@yopmail.com",
                    UserName = "dev@yopmail.com",
                    LastName = "Ken",
                    Id = "2a8fdbe3-14a4-0cda-8d36-39fa18266952",
                    PhoneNumber = "1234567890",
                },
                new User
                {
                    FirstName = "Paul",
                    Email = "paul@yopmail.com",
                    UserName = "paul@yopmail.com",
                    LastName = "Oke",
                    Id = "36145257-b784-496f-b012-ceb07e557e08",
                    PhoneNumber = "0123456789",
                }
            };
        }

        public static List<Organization> GetOrganizations()
        {
            return new List<Organization>
            {
                new Organization
                {
                    OrgId = Guid.Parse("d6b44d21-d94b-4716-acaa-f26a6328fa0b"),
                    Name = "Ekene's Organization",
                    Description = "One Hundred Years of Solitude is the first piece of literature since the Book of Genesis that should be required reading for the entire human race. . . . Mr. Garcia Marquez has done nothing less than to create in the reader a sense of all that is profound, meaningful, and meaningless in life.",
                },

                new Organization
                {
                    OrgId = Guid.Parse("8bea3613-18a2-4bd6-a8a3-6e34ce4881f3"),
                    Name = "Paul's Organization",
                    Description = "A hit HBO original series, Watchmen, the groundbreaking series from award-winning author Alan Moore, presents a world where the mere presence of American superheroes changed history--the U.S. won the Vietnam War, Nixon is still president, and the Cold War is in full effect.",
                },
                //shared with Paul
                new Organization
                {
                    OrgId = Guid.Parse("f07f8895-b747-459d-ceb1-85f77c5e7d67"),
                    Name = "Alan Moore",
                    Description = "After the sinking of a cargo ship, a solitary lifeboat remains bobbing on the wild blue Pacific. The only survivors from the wreck are a sixteen-year-old boy named Pi, a hyena, a wounded zebra, an orangutan—and a 450-pound Royal Bengal tiger.\r\n\r\nSoon the tiger has dispatched all but Pi Patel, whose fear, knowledge, and cunning allow him to coexist with the tiger, Richard Parker, for 227 days while lost at sea. When they finally reach the coast of Mexico, Richard Parker flees to the jungle, never to be seen again. The Japanese authorities who interrogate Pi refuse to believe his story and press him to tell them \"the truth.\" After hours of coercion, Pi tells a second story, a story much less fantastical, much more conventional—but is it more true?\r\n\r\n\"A story to make you believe in the soul-sustaining power of fiction.\"—Los Angeles Times Book Review\r\n",
                }
            };
        }

        public static List<UserOrganization> GetUsersOrganization()
        {
            return new List<UserOrganization>
            {
                //Ekene alone
                new UserOrganization
                {
                    UserOrgId = Guid.Parse("17e9a8de-678f-45a8-8ebb-3da3b5e789a6"),
                    Id = "2a8fdbe3-14a4-0cda-8d36-39fa18266952",
                    OrgId = Guid.Parse("d6b44d21-d94b-4716-acaa-f26a6328fa0b")
                },
                //Paul alone
                new UserOrganization
                {
                    UserOrgId = Guid.Parse("50783544-8d3a-4812-9053-74aa9027973c"),
                    Id = "36145257-b784-496f-b012-ceb07e557e08",
                    OrgId = Guid.Parse("8bea3613-18a2-4bd6-a8a3-6e34ce4881f3")
                },
                
                new UserOrganization
                {
                    UserOrgId = Guid.Parse("af9850ee-1275-4b48-a383-b3b9256f0ce4"),
                    Id = "2a8fdbe3-14a4-0cda-8d36-39fa18266952",
                    OrgId = Guid.Parse("f07f8895-b747-459d-ceb1-85f77c5e7d67")
                },

                new UserOrganization
                {
                    UserOrgId = Guid.Parse("d397d10a-a55b-4a62-8d2c-74ca3c72ffdb"),
                    Id =  "36145257-b784-496f-b012-ceb07e557e08",
                    OrgId = Guid.Parse("f07f8895-b747-459d-ceb1-85f77c5e7d67")
                }
            };
        }
    }
}
