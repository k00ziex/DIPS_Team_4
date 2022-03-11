from bully import Messages, Process
import uuid

class OriginalBullyProcess(Process):
    nonTimedOutHigherNeighbors = []
    allHigherNeighbors = []
    lowerNeighbors = []
    messagesSent = []


    def __init__(self, id, messagesSent=[0]) -> None:
        self.isAlive = True
        self.id = id
        self.messagesSent = 0
        self.messagesSent = messagesSent
        self.allHigherNeighbors = []
        self.lowerNeighbors = []
        self.nonTimedOutHigherNeighbors = []
        self.currentElectionID = -999
        self.hasSentCoordinator = False


    def receiveMessage(self, message, senderId, electionID):
        if(self.isAlive == True):
            if(message == Messages.Election):
                if(self.id > self.neighbors[0].id): #Used to check if we are the highest node in the system
                    # Send coordinator message if we are the highest ID
                    self.sendMessage(Messages.Answer, senderId, electionID)

                    # If the electionID is different from the one we've already worked on,
                    # we have not sent the coordinator message and this is our new "current" election
                    if(self.currentElectionID != electionID):
                        self.hasSentCoordinator = False
                        self.currentElectionID = electionID
                        
                    # If we haven't sent a coordinator message for this election, we send one
                    if(self.hasSentCoordinator == False):
                        for process in self.neighbors:
                            self.whoseLeader = self.id
                            self.sendMessage(Messages.Coordinator, process.id, electionID)
                            self.hasSentCoordinator = True

                else:
                    self.sendMessage(Messages.Answer, senderId, electionID) 
                    if(self.currentElectionID != electionID):
                        self.currentElectionID = electionID
                        self.startElection(setElectionID=False)
                            
            elif(message == Messages.Coordinator):
                self.whoseLeader = senderId

            elif(message == Messages.Answer):
                # Do nothing if we receive an answer
                pass

            elif(message == Messages.Timeout):
                if(self.nonTimedOutHigherNeighbors.count(senderId) > 0):
                    self.nonTimedOutHigherNeighbors.remove(senderId)
                
                if(len(self.nonTimedOutHigherNeighbors) <= 0): # If all our higher neighbors are timed out, we are the leader
                    self.whoseLeader = self.id
                    for neighborId in self.lowerNeighbors:
                        self.sendMessage(Messages.Coordinator, neighborId, self.currentElectionID)

            else:
                raise Exception("RECEIVEMESSAGE() EXCEPTION! PANIC!!!")
        else:
            for process in self.neighbors:
                if(process.id == senderId):
                    self.sendMessage(Messages.Timeout, process.id, electionID)
                    break
        

    def sendMessage(self, message, receiverId, electionID):
        for process in self.neighbors:
            if(process.id == receiverId):
                process.receiveMessage(message, self.id, electionID)
                # Count up the total number of messages sent
                if(message != Messages.Timeout): self.messagesSent[0] = self.messagesSent[0] + 1


    def setOtherProcessIDs(self, processList):
        self.neighbors = sorted(processList, reverse=True, key=lambda x: x.id)
        self.neighbors.remove(self)


    def startElection(self, setElectionID = True):
        if(setElectionID == True):
            self.currentElectionID = uuid.uuid4()

        self.splitNeighbors()

        if(len(self.allHigherNeighbors) > 0):
            for processId in self.allHigherNeighbors:
                self.sendMessage(Messages.Election, processId, self.currentElectionID)
        else: # If we have no higher neighbors, we elect ourself as leader
            self.whoseLeader = self.id
            if(len(self.lowerNeighbors) > 0):
                for processId in self.lowerNeighbors:
                    self.sendMessage(Messages.Coordinator, processId, self.currentElectionID)


            
    def splitNeighbors(self):
        self.allHigherNeighbors.clear()
        self.nonTimedOutHigherNeighbors.clear()
        for neighbor in self.neighbors:
            if(neighbor.id > self.id):
                self.nonTimedOutHigherNeighbors.append(neighbor.id)
                self.allHigherNeighbors.append(neighbor.id)
            elif(neighbor.id < self.id):
                self.lowerNeighbors.append(neighbor.id)


    def killNode(self):
        self.isAlive = False


    def startNode(self):
        self.isAlive = True
        self.startElection()


    def isLeader(self):
        return self.id == self.whoseLeader
