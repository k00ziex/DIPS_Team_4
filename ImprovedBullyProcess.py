from enum import Enum
import operator

class ImprovedBullyProcess:
    id = int
    whoseLeader = int
    networkMessagesCounter = int

    neighbors = []
    messages = []

    isAlive = bool

    def __init__(self, id, networkMessagesCounter):
        self.isAlive = True
        self.id = id
        self.messages = []
        self.networkMessagesCounter = networkMessagesCounter

    def setNeighbors(self, list_neighbors):
        self.neighbors = [x for x in list_neighbors if x.id != self.id]

    def receiveMessage(self, message, process):

        if not self.isAlive:
            return Messages.Timeout

        if message == Messages.Election:
            if self.id > process.id:
                self.sendMessage(Messages.ACK, process)
                self.whoseLeader = self.id
                for p in self.neighbors:
                    self.sendMessage(Messages.Coordinator, p)
                return Messages.ACK
            else:
                raise Exception("Sort Failed!")

        elif message == Messages.ACK:
            return Messages.ACK

        elif message == Messages.Coordinator:
            if self.id < process.id:
                self.whoseLeader = process.id
            else:
                print(f"Self: {self.id}, Process: {process.id}")
                raise Exception("Actually i should be the boss! This situation happens if high ID node dies and comes back live!")
            return Messages.ACK

        elif message == Messages.Timeout:
            raise Exception("Timeout should not be \"sent\"")
        else:
            raise Exception("YIKES, WHAT HAVE U DONE!!!!!")

    def sendMessage(self, message, process):
        self.networkMessagesCounter[0] = self.networkMessagesCounter[0] + 1
        return process.receiveMessage(message, self)

    def startElection(self):
        # Get ids higher than self in descending order
        higherids = [x for x in self.neighbors if x.id > self.id]
        higherids.sort(key=operator.attrgetter("id"), reverse=True)

        # Assume self to be leader
        self.whoseLeader = self.id

        # If there are no higher ids, Coordinate msg to all
        if len(higherids) <= 0:
            for p in self.neighbors:
                self.sendMessage(Messages.Coordinator, p)

        # Send Election msg to all processes with higher id, one at a time starting with the highest.
        noAnswer = True
        for iProcess in higherids:
            m = self.sendMessage(Messages.Election, iProcess)

            # If we do NOT receive a timeout, a leader has been found.
            if m != Messages.Timeout:
                noAnswer = False
                break

        # If we received timeout from everyone higher than self, we are the leader.
        if noAnswer:
            for p in self.neighbors:
                self.sendMessage(Messages.Coordinator, p)

    def killNode(self):
        self.isAlive = False

    def startNode(self):
        self.isAlive = True

    def isLeader(self):
        return self.id == self.whoseLeader


class Messages(Enum):
    Election = 1
    ACK = 2
    Coordinator = 3
    Timeout = 4
