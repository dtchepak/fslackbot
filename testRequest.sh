#!/bin/sh

curl \
    -d "token=2OMF6jGOFHgQm6ANRZFswxHi" \
    -d "team_id=T0001" \
    -d "channel_id=C2147483705" \
    -d "channel_name=test" \
    -d "timestamp=1355517523.000005" \
    -d "user_id=U2147483697" \
    -d "user_name=Steve" \
    -d "text=googlebot: look at ABC-123, DEF-456, ABC-123 and OTHER-03" \
    -d "trigger_word=googlebot:" \
    "http://localhost:48213/jira?jiraPath=http%3A%2F%2Flocalhost%2Ftest/&jiraProjects=ABC,DEF"

