from enum import Enum
import copy

class ImprovedBullyProcess:
    id = int
    whoseLeader = int

    neighbors = []
    messages = []

    isAlive = bool

    def __init__(self, id):
        self.isAlive = True
        self.id = id
        self.messages = []
        # Initial leader is missing keep in mind

    def setNeighbors(self, list_neighbors):
        self.neighbors = [x for x in list_neighbors if x.id != self.id]

    def receiveMessage(self, message, process):

        if not self.isAlive:
            return Messages.Timeout

        if message == Messages.Election:
            if self.id > process.id:
                self.sendMessage(Messages.Answer, process)
                self.whoseLeader = self.id
                for p in self.neighbors:
                    self.sendMessage(Messages.Coordinator, p)
                return Messages.Answer
            else:
                raise Exception("Sort Failed!")
        elif message == Messages.Answer:
            # This would be the ack to any message
            return Messages.Answer
        elif message == Messages.Coordinator:
            if self.id < process.id:
                self.whoseLeader = process.id
            else:
                raise Exception("Sort Failed!")
            return Messages.Answer
        elif message == Messages.Timeout:
            raise Exception("Timeout should not be \"sent\"")
        else:
            raise Exception("YIKES, WHAT HAVE U DONE!!!!!")

    def sendMessage(self, message, process):
        return process.receiveMessage(message, self)

    def startElection(self):
        higherids = [x for x in self.neighbors if x.id > self.id]
        higherids.sort(key=id, reverse=False)

        self.whoseLeader = self.id
        if len(higherids) <= 0:
            for p in self.neighbors:
                self.sendMessage(Messages.Coordinator, p)
        noAnswer = True
        for iProcess in higherids:
            m = self.sendMessage(Messages.Election, iProcess)
            if m != Messages.Timeout:
                noAnswer = False
                break

        if noAnswer:
            for p in self.neighbors:
                self.sendMessage(Messages.Coordinator, p)
            # self.whoseLeader is overwritten in receive if leader is found.




    def killNode(self):
        self.isAlive = False

    def startNode(self):
        self.isAlive = True

    def isLeader(self):
        return self.id == self.whoseLeader


class Messages(Enum):
    Election = 1
    Answer = 2
    Coordinator = 3
    Timeout = 4
