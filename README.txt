```
Author:     Draeden Jensen and John Haraden
Start Date: 03-23-2023
Course:     CS 3500, University of Utah, School of Computing
GitHub ID:  DraedenJensen
Repo:       https://github.com/uofu-cs3500-spring23/assignment-seven---chatting-caffeinatedcodersagain
Commit Date:04-03-2023 1:02 PM
Solution:   Networking_and_Logging
Copyright:  CS 3500, Draeden Jensen, and John Haraden 
            This work may not be copied for use in Academic Coursework.
```

# Overview of the solution's functionality

    This solution contains an implementation of a simple chat server. Four projects come together to build the overall functionality:

    - ChatServer: Defines the GUI for the server window. Allows a user to start and shut down a server, and await for clients
        to connect. A chat stream and a list of all connected participants are displayed in respective text windows, both of which
        are updated in real time.

    - ChatClient: Defines the GUI for the client window. Allows a user to join a remote server, and send messages to every
        connected client. A chat stream and a list of all connected participants are displayed in respective text windows. The chat
        is updated in real time, and the participant list can be updated by request.

    - Communications: Defines a Networking class which abstracts the complicated details of TCP connections. It defines methods for
        connecting and communicating over networks. Both of the above MAUI projects implement Networking objects, but this class can
        be applied to many different Networking needs.

    - FileLogger: Defines a custom logger which writes to a file. Used to log various events during the runtime of both GUIs.

# Additional Information / Design Decisions

    My implementation of the GUI could have been more readable/maintainable by making better use of the MVC architecture. I essentially 
    combined all 3 elements -- model, view, and controller -- into the MAUI classes, which I realize is not making good use of software
    practices. I had limited time given the complexity of this assignment (and the unexpected partnership collapse described below), so
    I left it in this state, which I figured was okay given that the professor had stated that better use of MVC was encouraged but optional.

    Both GUIs' functionality was tested thoroughly. I was a bit unclear on whether or not Unit tests were optional on this assignment, but in
    the end I went with Jo's Piazza post from March 31 clarifying that they were in fact not required. Given this, I left my projects without
    any Unit testing, however, given the extensive testing of both the client and server GUIs, I am confident that my code runs as inteded. More
    information on my testing process is detailed in the "Testing" section below.

    The two GUIs' graphical elements are set up very differetly, which I'll admit is a bit confusing upon looking at the code. The client's
    elements were defined almost entirely in C#, while the server's elements were defined almost entirely using XML. Setting up these GUIs
    was the one part of this assignment I did with my partner (John Haraden), and the difference in design here comes from us trading off
    driving/navigating -- I feel more comfortable using XML, while he prefers to use C# to code graphical elements.

    The code also could have been improved by adding log statements within the Networking class. Again, this was encouraged but optional,
    so I left it out. Given more time on the assignment, I certainly would've made the logging more thorough by instrumenting the Networking
    class.

    It's probably clear by now that I was very crunched for time on this assignment. A big part of that comes from the fact that a week into
    this assignment, my partnership collapsed and I had to finish most of the work on my own. More information will be detailed in the
    "Partnership Information" section below, but I talked to the professor and, given the circumstances, he was okay with me doing the bulk
    of this assignment alone. I'm sure I would've been able to get more done had I been working with a partner, but I feel comfortable knowing
    I got the assignment into a perfectly functional state despite this unexpected setback.

    Also, I feel like I should give credit to my dad, Mark Jensen, who is also a professional programmer. After losing my partner, I spent a 
    significant amount of time working with him to understand the complexities of the Networking class and how it is implemented in the GUIs
    here. All the code was either either writtten by me or obtained from the provided sample code, but his help went a long way towards my 
    understanding of the assignment.

# Partnership Information

    During the first week of this assignment, I worked with John Haraden as my partner. We worked together via pair programming to get the
    GUIs set up. Together, we got their layouts set up, but we didn't get any of the functionality working during this time. All of our work
    during this time was done via pair programming.

    As stated above, our partnership collapsed after the first week. John informed me that he will be withdrawing from the class and that the
    professor didn't want him turning in any more assignments given that. I spoke to the professor at this point and he told me that in light
    of these circumstances, I wouldn't lose points from doing the remainder of the assignment on my own.

    So that's what I did. The rest of the assignment was done solo by me. The file headers at the top of each file in the project are accurate;
    John's name is listed on only the part of the project he helped with. Just to reiterate, the only thing we did together was the layout for
    the GUI. Filling in the methods for the Networking class, adding functionality to the server and client GUIs, creating the CustomFileLogger
    class and instrumenting the GUIs, testing, and documenting the code was all done by me working on my own.

# Branching

    No additional branches were created during work on this assignment; all work was done on the main branch.

# Testing

    As stated above, I didn't have time to add Unit tests, so the functionality of the Networking class was tested after being implemented in the
    GUIs, by testing whether they worked as expected. My testing process was to test after coding; I built the core functionality for both GUIs and
    then tested them out to ensure that they worked properly. First, I tested my client code using the provided server executable file. I connected
    to the server, then debugged my client code using the results from testing various actions. In the end, I made sure it was working by thoroughly
    testing both basic functionality and any edge cases I could think of. Once it seemed to be working smoothly, I switched over to independently
    testing my server code. I connected multiple clients (using the provided client executable file) to my server, and tested in a similar process to
    the client code. Once this seemed to be working, I finally paired the two together, and used both my client and server code. At this point, both
    were working as intended, so I didn't run into any problems here. I tested and debugged the logger in a similar manner; manually running my client/
    server code and then opening the log file to ensure that the results were what I expected. Once I was satisfied with the results from every part
    of the project, I concluded my testing.

# Time Tracking

    Estimated time to complete (after reading assignment): 25 hours

    Time tracking (in hours):

        3/21 - 0
        3/22 - 3 (read specifications, created projects in solution, and built the client layout)
        3/23 - 3 (finished the clinet layout and built the server layout)
        3/24 - 0
        3/25 - 0
        3/26 - 0
        3/27 - 1 (partnership collapsed today, then I was mostly just confused and overwhelmed)
        3/28 - 5 (worked with my dad to understand things better, and completed the Networking class)
        3/29 - 1 (got confused and overwhelmed again, oops)
        3/30 - 4 (worked on my own to write and debug the client GUI, got it 90% functional)
        3/31 - 4 (finished the client GUI, then wrote and debugged the server GUI. Everything was working by the end)
        4/01 - 0
        4/02 - 2 (got the logger written and working)
        4/03 - 2 (cleaned up and documented all the code)

        Total: 25 hours

    I don't think my predictions are getting noticeably better/worse. Granted, my prediction happened to be right on the money for this assignment, but
    that feels more like a lucky guess than my abilities getting much better. Since the start of this class, I think I've been able to make fairly
    accurate time predictions, and I think that's stayed mostly consistent -- I'll be off by an hour or two every time. I think this shows that I have a
    pretty good understanding of my abilities. When I look over an assignment, I have a good feel of how long it will take me and how long will be spent
    figuring things out. As for my abilities themselves, I do sometimes feel insecure and feel like through time tracking, I'm able to see that relatively
    simple problems sometimes take me quite a long time to figure out. I get it figured out in the end, though, and I know sinking hours into fixing one
    little thing is a common issue for programmers, so I feel okay about how long everything is taking me. I'd just say my main goal for the future is to
    be able to spend less time feeling stuck during the debugging process.

# References

    1. Provided networking sample code
    2. Lab 10 sheet
    3. https://stackoverflow.com/questions/14998595/need-to-get-a-string-after-a-word-in-a-string-in-c-sharp
    4. https://learn.microsoft.com/en-us/dotnet/api/microsoft.extensions.logging.loggerextensions.beginscope?view=dotnet-plat-ext-7.0
    5. https://support.microsoft.com/en-us/windows/view-hidden-files-and-folders-in-windows-97fbc472-c603-9d90-91d0-1166d1d9f4b5
    6. https://learn.microsoft.com/en-us/dotnet/api/system.datetime.now?view=net-8.0
    7. https://learn.microsoft.com/en-us/dotnet/maui/user-interface/controls/editor?view=net-maui-7.0
    8. https://stackoverflow.com/questions/6803073/get-local-ip-address
    9. https://stackoverflow.com/questions/31637497/how-to-handle-socket-exception