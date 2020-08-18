using System;

namespace InfoEducatie.Contest.Participants.Participant
{
    [Flags]
    public enum SentMailsState
    {
        None = 0,
        ActivationEmail = 1,
        ParticipationDiplomaEmailSend = 2,
        PrizeDiplomaEmailSent = 4,
        ActivationParticipation = ActivationEmail | ParticipationDiplomaEmailSend,
        ActivationPrize = ActivationEmail | PrizeDiplomaEmailSent,
        ParticipationPrize = ParticipationDiplomaEmailSend | PrizeDiplomaEmailSent,
        ActivationParticipationPrize = ActivationEmail | ParticipationDiplomaEmailSend | PrizeDiplomaEmailSent
    }
}